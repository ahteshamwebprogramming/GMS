using Dapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Rooms;
using GMS.Services.DBContext;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GMS.Services
{
    public class RatesRepository : DapperGenericRepository<Rates>, IRatesRepository
    {
        public RatesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
        public async Task<bool> UpdateRatesAsync(List<RatesDTO> rates)
        {
            IDbConnection connection = DbConnection;
            if (rates == null || !rates.Any())
            {
                return true;
            }
            if (connection.State == ConnectionState.Closed)
            { connection.Open(); }



            using var transaction = connection.BeginTransaction();

            try
            {

                var rateLookup = new HashSet<(int RoomTypeId, DateTime Date)>();
                foreach (var rate in rates)
                {
                    var key = (rate.RoomTypeId, rate.Date.Date);
                    if (!rateLookup.Add(key))
                    {
                        throw new InvalidOperationException(
                            $"Duplicate RoomTypeId={rate.RoomTypeId} and Date={rate.Date:yyyy-MM-dd} found in input.");
                    }
                }


                var newRates = rates.Where(r => r.Id == 0).ToList();
                if (newRates.Any())
                {
                    var existingRates = await connection.QueryAsync<(int RoomTypeId, DateTime Date)>(
                        @"SELECT RoomTypeId, Date 
                  FROM Rates 
                  WHERE RoomTypeId IN @RoomTypeIds 
                  AND Date IN @Dates",
                        new
                        {
                            RoomTypeIds = newRates.Select(r => r.RoomTypeId).Distinct().ToArray(),
                            Dates = newRates.Select(r => r.Date.Date).Distinct().ToArray()
                        },
                        transaction);

                    var existingSet = new HashSet<(int RoomTypeId, DateTime Date)>(existingRates);
                    foreach (var rate in newRates)
                    {
                        if (existingSet.Contains((rate.RoomTypeId, rate.Date.Date)))
                        {
                            throw new InvalidOperationException(
                                $"A rate already exists for RoomTypeId={rate.RoomTypeId} on Date={rate.Date:yyyy-MM-dd}");
                        }
                    }
                }


                const int batchSize = 1000;
                var updates = rates.Where(r => r.Id > 0).ToList();
                var inserts = newRates;


                if (updates.Any())
                {
                    const string updateQuery = @"
                UPDATE Rates 
                SET Price = @Price, MinRate = @MinRate, MaxRate = @MaxRate 
                WHERE Id = @Id AND RoomTypeId = @RoomTypeId";

                    foreach (var batch in updates.Chunk(batchSize))
                    {
                        await connection.ExecuteAsync(updateQuery, batch, transaction);
                    }
                }


                if (inserts.Any())
                {
                    const string insertQuery = @"
                INSERT INTO Rates (RoomTypeId, Date, Price, MinRate, MaxRate)
                VALUES (@RoomTypeId, @Date, @Price, @MinRate, @MaxRate)";

                    foreach (var batch in inserts.Chunk(batchSize))
                    {
                        await connection.ExecuteAsync(insertQuery, batch, transaction);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task UpdateBulkInventoryAsync(BulkUpdateViewModel model)
        {
            IDbConnection connection = DbConnection;

            //using var connection = _connectionFactory.CreateConnection();
            if (connection.State == ConnectionState.Closed)
            { connection.Open(); }
            using var transaction = connection.BeginTransaction();

            try
            {
                var query = @"
                MERGE INTO RoomInventory AS target
                USING (SELECT @RtypeID AS RtypeID, @Date AS Date, @TotalRoomForSale AS TotalRoomForSale,@PlanId as PlanId) 
                  AS source
                ON target.RtypeID = source.RtypeID
                   AND target.PlanId=source.PlanId
                   AND target.Date = source.Date
                WHEN MATCHED THEN
                    UPDATE SET TotalRoomForSale = source.TotalRoomForSale
                WHEN NOT MATCHED THEN
                    INSERT (RtypeID, Date, TotalRoomForSale,PlanId)
                    VALUES (source.RtypeID, source.Date, source.TotalRoomForSale,source.PlanId);";


                var selectedInventory = model.Inventory.Where(r => r.IsSelected).ToList();
                Console.WriteLine($"Processing {selectedInventory.Count} selected inventory items");
                var selectedDays = model.SelectedDaysList?.Split(',')?.ToList() ?? new List<string>();

                foreach (var room in selectedInventory)
                {
                    if (room.RoomTypeId <= 0)
                    {
                        Console.WriteLine($"Skipping invalid RoomTypeId: {room.RoomTypeId}");
                        continue;
                    }


                    var exists = await connection.ExecuteScalarAsync<int>(
                        "SELECT COUNT(*) FROM RoomType WHERE ID = @Id",
                        new { Id = room.RoomTypeId },
                        transaction);
                    if (exists == 0)
                    {
                        Console.WriteLine($"Error: RoomTypeId {room.RoomTypeId} not found in RoomType table.");
                        throw new Exception($"Invalid RoomTypeId: {room.RoomTypeId}");
                    }


                    for (var date = model.FromDate; date <= model.ToDate; date = date.AddDays(1))
                    {
                        // Skip dates that don't match selected days (unless "All" is selected)
                        if (selectedDays.Any() && !selectedDays.Contains("All"))
                        {
                            var dayName = date.DayOfWeek.ToString();
                            if (!selectedDays.Contains(dayName))
                            {
                                continue;
                            }
                        }


                        var rowsAffected = await connection.ExecuteAsync(query, new
                        {
                            RtypeID = room.RoomTypeId,
                            Date = date,
                            TotalRoomForSale = room.RoomsOpen ?? 0,
                            PlanId = model.ChannelId
                        }, transaction);
                        Console.WriteLine($"Rows affected for {room.RoomTypeId} on {date}: {rowsAffected}");
                    }
                }
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Database error: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateBulkRatesAsync(int channelId, DateTime fromDate, DateTime toDate, List<RoomRateBulkUpdate> rates)
        {
            //using var connection = _connectionFactory.CreateConnection();
            //connection.Open();

            IDbConnection connection = DbConnection;

            //using var connection = _connectionFactory.CreateConnection();
            if (connection.State == ConnectionState.Closed)
            { connection.Open(); }
            using var transaction = connection.BeginTransaction();

            try
            {
                const string query = @"
            MERGE INTO Rates AS target
            USING (VALUES (@RoomTypeId, @Date, @Price, @MinRate, @MaxRate, @PlanId)) 
                  AS source (RoomTypeId, Date, Price, MinRate, MaxRate,PlanId)
            ON target.RoomTypeId = source.RoomTypeId 
               AND target.PlanId = source.PlanId 
               AND target.Date = source.Date 
            WHEN MATCHED THEN
                UPDATE SET Price = source.Price,
                           MinRate = source.MinRate,
                           MaxRate = source.MaxRate,
                           PlanId = source.PlanId
            WHEN NOT MATCHED THEN
                INSERT (RoomTypeId, Date, Price, MinRate, MaxRate,PlanId)
                VALUES (source.RoomTypeId, source.Date, source.Price, source.MinRate, source.MaxRate, source.PlanId);";


                var rateData = new List<object>();
                var days = (toDate - fromDate).Days + 1;
                foreach (var rate in rates)
                {
                    for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                    {
                        rateData.Add(new
                        {
                            RoomTypeId = rate.RoomTypeId,
                            Date = date,
                            Price = rate.SaleRate,
                            MinRate = rate.MinimumRate,
                            MaxRate = rate.MaximumRate,
                            PlanId = channelId
                        });
                    }
                }


                const int batchSize = 1000;
                for (int i = 0; i < rateData.Count; i += batchSize)
                {
                    var batch = rateData.Skip(i).Take(batchSize);
                    await connection.ExecuteAsync(query, batch, transaction);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error updating bulk rates", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task UpdateBulkRatesAsyncPackages(int channelId, DateTime fromDate, DateTime toDate, List<RoomRateBulkUpdate> rates, string selectedDaysList)
        {
            //using var connection = _connectionFactory.CreateConnection();
            //connection.Open();

            IDbConnection connection = DbConnection;

            //using var connection = _connectionFactory.CreateConnection();
            if (connection.State == ConnectionState.Closed)
            { connection.Open(); }
            using var transaction = connection.BeginTransaction();

            try
            {
                const string query = @"
            MERGE INTO Rates AS target
            USING (VALUES (@RoomTypeId, @Date, @Price, @MinRate, @MaxRate, @PlanId)) 
                  AS source (RoomTypeId, Date, Price, MinRate, MaxRate,PlanId)
            ON target.RoomTypeId = source.RoomTypeId 
               AND target.PlanId = source.PlanId 
               AND target.Date = source.Date 
            WHEN MATCHED THEN
                UPDATE SET Price = source.Price,
                           MinRate = source.MinRate,
                           MaxRate = source.MaxRate,
                           PlanId = source.PlanId
            WHEN NOT MATCHED THEN
                INSERT (RoomTypeId, Date, Price, MinRate, MaxRate,PlanId)
                VALUES (source.RoomTypeId, source.Date, source.Price, source.MinRate, source.MaxRate, source.PlanId);";


                var rateData = new List<object>();
                var selectedDays = selectedDaysList?.Split(',')?.ToList() ?? new List<string>();
                //var days = (toDate - fromDate).Days + 1;
                foreach (var rate in rates)
                {
                    for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                    {
                        // Skip dates that don't match selected days (unless "All" is selected)
                        if (selectedDays.Any() && !selectedDays.Contains("All"))
                        {
                            var dayName = date.DayOfWeek.ToString();
                            if (!selectedDays.Contains(dayName))
                            {
                                continue;
                            }
                        }
                        rateData.Add(new
                        {
                            RoomTypeId = rate.RoomTypeId,
                            Date = date,
                            Price = rate.SaleRate,
                            MinRate = rate.MinimumRate,
                            MaxRate = rate.MaximumRate,
                            PlanId = channelId
                        });
                    }
                }


                const int batchSize = 1000;
                for (int i = 0; i < rateData.Count; i += batchSize)
                {
                    var batch = rateData.Skip(i).Take(batchSize);
                    await connection.ExecuteAsync(query, batch, transaction);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error updating bulk rates", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task SaveRoomPercentagesAsync(List<RoomPercentageData> percentageData, DateTime fromDate, DateTime toDate)
        {
            IDbConnection connection = DbConnection;
            if (connection.State == ConnectionState.Closed)
            { connection.Open(); }
            using var transaction = connection.BeginTransaction();

            try
            {
                // Create a temporary table to hold the percentage data
                var createTempTable = @"
            CREATE TABLE #TempPercentages (
                RoomTypeId INT,
                Percentage1 DECIMAL(5,4),
                Percentage2 DECIMAL(5,4),
                Percentage3 DECIMAL(5,4),
                Percentage4 DECIMAL(5,4),
                Percentage5 DECIMAL(5,4),
                Percentage6 DECIMAL(5,4),
                Percentage7 DECIMAL(5,4),
                Percentage8 DECIMAL(5,4)
            )";

                await connection.ExecuteAsync(createTempTable, transaction: transaction);

                // Bulk insert the percentage data into temp table
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction))
                {
                    bulkCopy.DestinationTableName = "#TempPercentages";
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("RoomTypeId", typeof(int));
                    for (int i = 1; i <= 8; i++)
                    {
                        dataTable.Columns.Add($"Percentage{i}", typeof(decimal));
                    }

                    foreach (var item in percentageData)
                    {
                        dataTable.Rows.Add(item.RoomTypeId,
                            item.Percentages[0], item.Percentages[1], item.Percentages[2],
                            item.Percentages[3], item.Percentages[4], item.Percentages[5],
                            item.Percentages[6], item.Percentages[7]);
                    }

                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                // Use a single MERGE statement with a date generator
                var mergeQuery = @"
            WITH DateRange AS (
                SELECT DATEADD(DAY, number, @FromDate) AS Date
                FROM master..spt_values
                WHERE type = 'P' AND number <= DATEDIFF(DAY, @FromDate, @ToDate)
            ),
            PercentageSlots AS (
                SELECT RoomTypeId, Date,
                       1 AS PercentageSlot, Percentage1 AS Percentage FROM #TempPercentages CROSS JOIN DateRange
                UNION ALL
                SELECT RoomTypeId, Date, 2, Percentage2 FROM #TempPercentages CROSS JOIN DateRange
                UNION ALL
                SELECT RoomTypeId, Date, 3, Percentage3 FROM #TempPercentages CROSS JOIN DateRange
                UNION ALL
                SELECT RoomTypeId, Date, 4, Percentage4 FROM #TempPercentages CROSS JOIN DateRange
                UNION ALL
                SELECT RoomTypeId, Date, 5, Percentage5 FROM #TempPercentages CROSS JOIN DateRange
                UNION ALL
                SELECT RoomTypeId, Date, 6, Percentage6 FROM #TempPercentages CROSS JOIN DateRange
                UNION ALL
                SELECT RoomTypeId, Date, 7, Percentage7 FROM #TempPercentages CROSS JOIN DateRange
                UNION ALL
                SELECT RoomTypeId, Date, 8, Percentage8 FROM #TempPercentages CROSS JOIN DateRange
            )
            MERGE INTO RoomTypePercentages WITH (HOLDLOCK) AS target
            USING PercentageSlots AS source
            ON (target.RoomTypeId = source.RoomTypeId 
                AND target.Date = source.Date 
                AND target.PercentageSlot = source.PercentageSlot)
            WHEN MATCHED THEN
                UPDATE SET target.Percentage = source.Percentage
            WHEN NOT MATCHED THEN
                INSERT (RoomTypeId, Date, Percentage, PercentageSlot)
                VALUES (source.RoomTypeId, source.Date, source.Percentage, source.PercentageSlot);";

                await connection.ExecuteAsync(
                    mergeQuery,
                    new { FromDate = fromDate, ToDate = toDate },
                    transaction
                );

                // Clean up
                await connection.ExecuteAsync("DROP TABLE #TempPercentages", transaction: transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task UpdateBulkRestrictionsAsync(int channelId, DateTime fromDate, DateTime toDate, List<RoomRestrictionBulkUpdate> restrictions)
        {
            IDbConnection connection = DbConnection;
            if (connection.State == ConnectionState.Closed)
            { connection.Open(); }

            using var transaction = connection.BeginTransaction();
            try
            {

                var restrictionData = new List<object>();
                var days = (toDate - fromDate).Days + 1;
                foreach (var restriction in restrictions)
                {
                    for (int i = 0; i < days; i++)
                    {
                        var currentDate = fromDate.AddDays(i);
                        restrictionData.Add(new
                        {
                            restriction.RoomTypeId,
                            Date = currentDate,
                            restriction.StopSell,
                            restriction.CloseOnArrival,
                            restriction.RestrictStay,
                            restriction.MinimumNights,
                            restriction.MaximumNights
                        });
                    }
                }


                await connection.ExecuteAsync(
                    @"MERGE RoomRestrictions AS target
              USING (VALUES (@RoomTypeId, @Date, @StopSell, @CloseOnArrival, @RestrictStay, @MinimumNights, @MaximumNights)) 
              AS source (RoomTypeId, Date, StopSell, CloseOnArrival, RestrictStay, MinimumNights, MaximumNights)
              ON target.RoomTypeId = source.RoomTypeId AND target.Date = source.Date
              WHEN MATCHED THEN
                  UPDATE SET StopSell = source.StopSell,
                             CloseOnArrival = source.CloseOnArrival,
                             RestrictStay = source.RestrictStay,
                             MinimumNights = source.MinimumNights,
                             MaximumNights = source.MaximumNights
              WHEN NOT MATCHED THEN
                  INSERT (RoomTypeId, Date, StopSell, CloseOnArrival, RestrictStay, MinimumNights, MaximumNights)
                  VALUES (source.RoomTypeId, source.Date, source.StopSell, source.CloseOnArrival, source.RestrictStay, source.MinimumNights, source.MaximumNights);",
                    restrictionData,
                    transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error updating restrictions", ex);
            }
        }

    }
}
