using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Rooms;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.Rooms;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rooms.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BulkRateAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkRateAPIController> _logger;
    private readonly IMapper _mapper;
    public BulkRateAPIController(IUnitOfWork unitOfWork, ILogger<BulkRateAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    public async Task<List<RoomInventoryViewModel>> GetRoomInventory(DateTime startDate, int days, int planId)
    {

        var endDate = startDate.AddDays(days - 1);

        var roomTypesQuery = @"SELECT 
                            rt.ID AS RoomTypeId, 
                            rt.RType AS RoomTypeName, 
                            COUNT(r.RNumber) AS TotalRooms,
                            COUNT(r.RNumber) AS RoomsAvailable
                            FROM RoomType rt
                            LEFT JOIN Rooms r ON r.RTypeID = rt.ID AND r.Status = 1
                            WHERE rt.Status = 1
                             GROUP BY rt.ID, rt.RType";

        var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomTypeViewModel>(roomTypesQuery);
        //var roomTypes1 = (await connection.QueryAsync<RoomTypeViewModel>(roomTypesQuery)).ToList();

        if (!roomTypes.Any()) return new List<RoomInventoryViewModel>();


        var inventoryQuery = @"SELECT Id,  RtypeID AS RoomTypeId,  Date,  TotalRoomForSale
                                FROM RoomInventory
                                WHERE RtypeID in @RoomTypeIDs
                                  AND PlanId = @PlanId
                                  AND Date = 
                                      (
                                        SELECT TOP 1 Date
                                        FROM RoomInventory
                                        WHERE RtypeID in @RoomTypeIDs
                                          AND PlanId = @PlanId
                                          AND (Date = @StartDate 
                                               OR Date = (SELECT MAX(Date) 
                                                          FROM RoomInventory 
                                                          WHERE RtypeID in @RoomTypeIDs
                                                            AND PlanId = @PlanId))
                                        ORDER BY CASE WHEN Date = @StartDate THEN 1 ELSE 2 END
                                      );";

        var inventoryDataRaw = await _unitOfWork.GenOperations.GetTableData<DailyRoomInventory>(inventoryQuery, new { @PlanId = planId, StartDate = startDate, EndDate = endDate, RoomTypeIDs = roomTypes.Select(rt => rt.RoomTypeId).ToArray() });
        //var inventoryData = inventoryDataRaw.ToLookup(x => (x.RoomTypeId, x.Date.Date));
        var inventoryData = inventoryDataRaw.ToLookup(x => (x.RoomTypeId));

        //var inventoryData1 = (await connection.QueryAsync<DailyRoomInventory>(
        //    inventoryQuery,
        //    new { StartDate = startDate, EndDate = endDate, RoomTypeIDs = roomTypes.Select(rt => rt.RoomTypeId).ToArray() }
        //)).ToLookup(x => (x.RoomTypeId, x.Date.Date));

        var result = new List<RoomInventoryViewModel>();

        foreach (var roomType in roomTypes)
        {
            var dailyInventory = new List<DailyRoomInventory>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                //var inventoryRecord = inventoryData[(roomType.RoomTypeId, date)].FirstOrDefault();
                var inventoryRecord = inventoryData[(roomType.RoomTypeId)].FirstOrDefault();

                dailyInventory.Add(new DailyRoomInventory
                {
                    Id = inventoryRecord?.Id ?? 0,
                    RoomTypeId = roomType.RoomTypeId,
                    Date = date,
                    TotalRooms = roomType.TotalRooms,
                    RoomsAvailable = roomType.RoomsAvailable,
                    TotalRoomForSale = inventoryRecord?.TotalRoomForSale ?? 0
                });
            }

            result.Add(new RoomInventoryViewModel
            {
                RtypeID = roomType.RoomTypeId,
                RoomTypeName = roomType.RoomTypeName,
                DailyInventory = dailyInventory
            });
        }

        return result;
    }


    public async Task<List<RoomRateViewModel>> GetRoomRatesAsync(DateTime startDate, int days)
    {
        //using var connection = _connectionFactory.CreateConnection();
        var endDate = startDate.AddDays(days - 1);
        var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomTypeDTO>("SELECT * FROM RoomType WHERE Status = 1");
        //var roomTypes1 = await connection.QueryAsync<RoomType>(
        //    "SELECT * FROM RoomType WHERE Status = 1");

        var result = new List<RoomRateViewModel>();

        foreach (var roomType in roomTypes)
        {
            var rates = await _unitOfWork.GenOperations.GetTableData<RatesDTO>(
                @"SELECT Id, RoomTypeId, Date, Price ,MinRate, MaxRate
              FROM Rates 
              WHERE RoomTypeId = @RoomTypeId 
              AND Date BETWEEN @StartDate AND @EndDate",
                new { RoomTypeId = roomType.Id, StartDate = startDate, EndDate = endDate });

            //var rates1 = await connection.QueryAsync<Rate>(
            //    @"SELECT Id, RoomTypeId, Date, Price ,MinRate, MaxRate
            //  FROM Rates 
            //  WHERE RoomTypeId = @RoomTypeId 
            //  AND Date BETWEEN @StartDate AND @EndDate",
            //    new { RoomTypeId = roomType.Id, StartDate = startDate, EndDate = endDate });

            result.Add(new RoomRateViewModel
            {
                RoomTypeId = roomType.Id,
                RoomTypeName = roomType.Rtype,
                DailyRates = rates.ToList()
            });
        }

        return result;
    }

    public async Task<List<RoomRateViewModel>> GetRoomRatesAsync(DateTime startDate, int days, int planId)
    {
        //using var connection = _connectionFactory.CreateConnection();
        var endDate = startDate.AddDays(days - 1);
        var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomTypeDTO>("SELECT * FROM RoomType WHERE Status = 1");
        //var roomTypes1 = await connection.QueryAsync<RoomType>(
        //    "SELECT * FROM RoomType WHERE Status = 1");

        var result = new List<RoomRateViewModel>();

        foreach (var roomType in roomTypes)
        {
            var rates = await _unitOfWork.GenOperations.GetTableData<RatesDTO>(
                @"SELECT Id, RoomTypeId, Date, Price, MinRate, MaxRate
                        FROM Rates
                        WHERE RoomTypeId = @RoomTypeId 
                          AND PlanId = @PlanId
                          AND Date = 
                              (
                                SELECT TOP 1 Date
                                FROM Rates
                                WHERE RoomTypeId = @RoomTypeId
                                  AND PlanId = @PlanId
                                  AND (Date = @StartDate 
                                       OR Date = (SELECT MAX(Date) 
                                                  FROM Rates 
                                                  WHERE RoomTypeId = @RoomTypeId 
                                                    AND PlanId = @PlanId))
                                ORDER BY CASE WHEN Date = @StartDate THEN 1 ELSE 2 END
                              );",
                                        new { RoomTypeId = roomType.Id, StartDate = startDate, @PlanId = planId });

            //var rates1 = await connection.QueryAsync<Rate>(
            //    @"SELECT Id, RoomTypeId, Date, Price ,MinRate, MaxRate
            //  FROM Rates 
            //  WHERE RoomTypeId = @RoomTypeId 
            //  AND Date BETWEEN @StartDate AND @EndDate",
            //    new { RoomTypeId = roomType.Id, StartDate = startDate, EndDate = endDate });

            result.Add(new RoomRateViewModel
            {
                RoomTypeId = roomType.Id,
                RoomTypeName = roomType.Rtype,
                DailyRates = rates.ToList()
            });
        }

        return result;
    }
    public async Task<List<RoomIncrementData>> GetRoomIncrementDataAsync(DateTime startDate, int days, DateTime fromDate, DateTime toDate)
    {
        //using var connection = _connectionFactory.CreateConnection();
        var endDate = startDate.AddDays(days - 1);

        var roomQuery = @"
        SELECT 
            rt.ID AS RoomTypeId, 
            rt.RType AS RoomTypeName, 
            COUNT(r.RNumber) AS NumberOfRooms
        FROM RoomType rt
        LEFT JOIN Rooms r ON r.RTypeID = rt.ID AND r.Status = 1
        WHERE rt.Status = 1
        GROUP BY rt.ID, rt.RType";

        var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomIncrementData>(roomQuery);
        //var roomTypes1 = (await connection.QueryAsync<RoomIncrementData>(roomQuery)).ToList();

        if (!roomTypes.Any())
        {
            return new List<RoomIncrementData>();
        }

        var percentageQuery = @"
        SELECT 
            RoomTypeId,
            Date,
            Percentage,
            PercentageSlot
        FROM RoomTypePercentages
        WHERE RoomTypeId IN @RoomTypeIds
        AND Date BETWEEN @FromDate AND @ToDate";
        var percentages = await _unitOfWork.GenOperations.GetTableData<RoomTypePercentageEntity>(percentageQuery, new
        {
            RoomTypeIds = roomTypes.Select(rt => rt.RoomTypeId).ToArray(),
            FromDate = fromDate,
            ToDate = toDate
        });
        //var percentages1 = (await connection.QueryAsync<RoomTypePercentageEntity>(
        //    percentageQuery,
        //    new
        //    {
        //        RoomTypeIds = roomTypes.Select(rt => rt.RoomTypeId).ToArray(),
        //        FromDate = fromDate,
        //        ToDate = toDate
        //    }
        //)).ToList();

        // Group percentages by RoomTypeId and Date, then pivot to array
        foreach (var room in roomTypes)
        {
            var roomPercentages = percentages
                .Where(p => p.RoomTypeId == room.RoomTypeId && p.Date == fromDate) // Assuming we show one day's data
                .OrderBy(p => p.PercentageSlot)
                .Select(p => p.Percentage)
                .ToList();

            if (roomPercentages.Any())
            {
                room.Percentages = roomPercentages.Count == 8
                    ? roomPercentages
                    : roomPercentages.Concat(new decimal[] { -0.10m, 0m, 0.25m, 0.50m, 0.35m, 0.20m, 0.10m, 0.75m }.Skip(roomPercentages.Count)).Take(8).ToList();
            }
            else
            {
                room.Percentages = new List<decimal> { -0.10m, 0m, 0.25m, 0.50m, 0.35m, 0.20m, 0.10m, 0.75m };
            }
        }

        return roomTypes;
    }
    public async Task<List<RoomRestrictionViewModel>> GetRoomRestrictionsAsync(DateTime startDate, int days)
    {
        //using var connection = _connectionFactory.CreateConnection();
        var endDate = startDate.AddDays(days - 1);

        var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomTypeDTO>("SELECT * FROM RoomType WHERE Status = 1");
        //    var roomTypes1 = await connection.QueryAsync<RoomType>(
        //"SELECT * FROM RoomType WHERE Status = 1");

        var result = new List<RoomRestrictionViewModel>();

        var restrictionsQuery = await _unitOfWork.GenOperations.GetTableData<RoomRestriction>(@"SELECT 
            RoomTypeId, 
            Date, 
            StopSell, 
            CloseOnArrival, 
            RestrictStay, 
            MinimumNights, 
            MaximumNights
        FROM RoomRestrictions 
        WHERE Date BETWEEN @StartDate AND @EndDate",
            new { StartDate = startDate, EndDate = endDate });

        //var restrictionsQuery1 = await connection.QueryAsync<RoomRestriction>(
        //    @"SELECT 
        //    RoomTypeId, 
        //    Date, 
        //    StopSell, 
        //    CloseOnArrival, 
        //    RestrictStay, 
        //    MinimumNights, 
        //    MaximumNights
        //FROM RoomRestrictions 
        //WHERE Date BETWEEN @StartDate AND @EndDate",
        //    new { StartDate = startDate, EndDate = endDate });

        var restrictionsByRoomType = restrictionsQuery.ToLookup(r => r.RoomTypeId);

        foreach (var roomType in roomTypes)
        {

            var restriction = restrictionsByRoomType[roomType.Id].FirstOrDefault(r => r.Date == startDate);

            result.Add(new RoomRestrictionViewModel
            {
                RoomTypeId = roomType.Id,
                RoomTypeName = roomType.Rtype,
                DailyRestrictions = new List<RoomRestriction>
            {
                new RoomRestriction
                {
                    RoomTypeId = roomType.Id,
                    Date = startDate,
                    StopSell = restriction?.StopSell,
                    CloseOnArrival = restriction?.CloseOnArrival,
                    RestrictStay = restriction?.RestrictStay,
                    MinimumNights = restriction?.MinimumNights,
                    MaximumNights = restriction?.MaximumNights
                }
            }
            });
        }

        return result;
    }


    public async Task<bool> UpdateBulkInventoryAsync1(int channelId, DateTime fromDate, DateTime toDate, List<RoomInventoryBulkUpdate> inventory)
    {
        try
        {
            //await _unitOfWork.Rates.UpdateBulkInventoryAsync(channelId, fromDate, toDate, inventory);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateBulkInventoryAsync)}");
            throw;
        }
    }
    
    public async Task<bool> UpdateBulkInventoryAsync(BulkUpdateViewModel model)
    {
        try
        {
            await _unitOfWork.Rates.UpdateBulkInventoryAsync(model);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateBulkInventoryAsync)}");
            throw;
        }
    }
    public async Task<bool> UpdateBulkRatesAsync(int channelId, DateTime fromDate, DateTime toDate, List<RoomRateBulkUpdate> rates)
    {
        try
        {
            await _unitOfWork.Rates.UpdateBulkRatesAsync(channelId, fromDate, toDate, rates);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateBulkRatesAsync)}");
            throw;
        }
    }

    public async Task<bool> UpdateBulkRatesAsync_Packages(int channelId, DateTime fromDate, DateTime toDate, List<RoomRateBulkUpdate> rates, string selectedDaysList)
    {
        try
        {
            await _unitOfWork.Rates.UpdateBulkRatesAsyncPackages(channelId, fromDate, toDate, rates, selectedDaysList);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateBulkRatesAsync)}");
            throw;
        }
    }
    public async Task SaveRoomPercentagesAsync(List<RoomPercentageData> percentageData, DateTime fromDate, DateTime toDate)
    {
        try
        {
            await _unitOfWork.Rates.SaveRoomPercentagesAsync(percentageData, fromDate, toDate);
            //return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateBulkRatesAsync)}");
            throw;
        }
    }
    public async Task UpdateBulkRestrictionsAsync(int channelId, DateTime fromDate, DateTime toDate, List<RoomRestrictionBulkUpdate> restrictions)
    {
        try
        {
            await _unitOfWork.Rates.UpdateBulkRestrictionsAsync(channelId, fromDate, toDate, restrictions);
            //return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateBulkRatesAsync)}");
            throw;
        }
    }
}

