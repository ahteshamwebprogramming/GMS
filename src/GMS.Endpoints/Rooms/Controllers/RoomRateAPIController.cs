using AutoMapper;
using Dapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Rooms;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Rooms;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rooms.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomRateAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomRateAPIController> _logger;
    private readonly IMapper _mapper;
    public RoomRateAPIController(IUnitOfWork unitOfWork, ILogger<RoomRateAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            string query = "SELECT * FROM RoomType WHERE Status = 1";
            var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomType>(query);
            return Ok(roomTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllAsync)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAllRatePlans()
    {
        try
        {
            string query = @"Select * from RatePlans where IsActive=1";
            var ratePlans = await _unitOfWork.GenOperations.GetTableData<RatePlansDTO>(query);
            return Ok(ratePlans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllAsync)}");
            throw;
        }
    }
    public async Task<List<RoomRateViewModel>> GetRoomRatesAsync(DateTime startDate, int days, int PlanId)
    {
        try
        {
            var endDate = startDate.AddDays(days - 1);

            string roomTypesquery = "SELECT * FROM RoomType WHERE Status = 1";
            var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomTypeDTO>(roomTypesquery);
            var result = new List<RoomRateViewModel>();

            foreach (var roomType in roomTypes)
            {
                string rateQuery = @"SELECT Id, RoomTypeId, Date, Price ,MinRate, MaxRate
                                  FROM Rates 
                                  WHERE RoomTypeId = @RoomTypeId and PlanId=@PlanId
                                  AND Date BETWEEN @StartDate AND @EndDate";
                var rateParam = new { RoomTypeId = roomType.Id, StartDate = startDate, EndDate = endDate, @PlanId = PlanId };
                var rates = await _unitOfWork.GenOperations.GetTableData<RatesDTO>(rateQuery, rateParam);

                result.Add(new RoomRateViewModel
                {
                    RoomTypeId = roomType.Id,
                    RoomTypeName = roomType.Rtype,
                    DailyRates = rates.ToList()

                });
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllAsync)}");
            throw;
        }
    }
    public async Task<List<RoomRateViewModel>> GetRoomRatesAsync(DateTime startDate, int days)
    {
        try
        {
            var endDate = startDate.AddDays(days - 1);

            string roomTypesquery = "SELECT * FROM RoomType WHERE Status = 1";
            var roomTypes = await _unitOfWork.GenOperations.GetTableData<RoomTypeDTO>(roomTypesquery);
            var result = new List<RoomRateViewModel>();

            foreach (var roomType in roomTypes)
            {
                string rateQuery = @"SELECT Id, RoomTypeId, Date, Price ,MinRate, MaxRate
                                  FROM Rates 
                                  WHERE RoomTypeId = @RoomTypeId 
                                  AND Date BETWEEN @StartDate AND @EndDate";
                var rateParam = new { RoomTypeId = roomType.Id, StartDate = startDate, EndDate = endDate };
                var rates = await _unitOfWork.GenOperations.GetTableData<RatesDTO>(rateQuery, rateParam);

                result.Add(new RoomRateViewModel
                {
                    RoomTypeId = roomType.Id,
                    RoomTypeName = roomType.Rtype,
                    DailyRates = rates.ToList()
                });
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllAsync)}");
            throw;
        }
    }
    public async Task<(List<RoomInventoryViewModel> Rooms, int TotalRooms, decimal OccupancyPercentage)> GetRoomInventoryWithOccupancy(DateTime startDate, int days)
    {
        var endDate = startDate.AddDays(days - 1);

        var roomTypesQuery = @"SELECT 
                                rt.ID AS RtypeID, 
                                rt.RType AS Name, 
                                COUNT(r.RNumber) AS TotalRooms
                            FROM RoomType rt
                            LEFT JOIN Rooms r ON r.RTypeID = rt.ID AND r.Status = 1
                            WHERE rt.Status = 1
                            GROUP BY rt.ID, rt.RType";
        var roomTypes = await _unitOfWork.GenOperations.GetTableData<CMRoomAvailability>(roomTypesQuery);

        int totalRooms = roomTypes.Sum(rt => rt.TotalRooms) ?? 0;

        var occupancyQuery = @"SELECT 
            r.RTypeID,
            CAST(ra.CheckInDate AS DATE) AS OccupancyDate,
            COUNT(1) AS BookedRooms
        FROM Rooms r
        INNER JOIN RoomAllocation ra ON ra.RNumber = r.RNumber
        WHERE r.Status = 1
        AND ra.IsActive = 1
        AND (
                    @StartDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)  
                    OR 
                    @EndDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)
                    OR
                    ISNULL(CheckInDate, FD) BETWEEN @StartDate AND @EndDate  
                    OR
                    ISNULL(CheckOutDate, TD) BETWEEN @StartDate AND @EndDate
        )
        GROUP BY r.RTypeID, CAST(ra.CheckInDate AS DATE)";
        var occupancyData = await _unitOfWork.GenOperations.GetTableData<CMOccupancyTable>(occupancyQuery, new { StartDate = startDate, EndDate = endDate });

        var occupancyLookup = occupancyData.ToDictionary(
            x => (x.RtypeID, x.OccupancyDate),
            x => x.BookedRooms,
            EqualityComparer<(int, DateTime)>.Default);

        var dateRange = Enumerable.Range(0, days).Select(d => startDate.AddDays(d));

        var result = roomTypes.Select(roomType =>
        {
            var dailyInventory = dateRange.Select(date =>
            {
                var bookedRooms = occupancyLookup.TryGetValue((roomType.RtypeID ?? 0, date), out var booked) ? booked : 0;
                var availableRooms = roomType.TotalRooms - bookedRooms;

                var dailyRecord = new DailyRoomInventory
                {
                    RoomTypeId = roomType.RtypeID ?? 0,
                    Date = date,
                    TotalRooms = roomType.TotalRooms ?? 0,
                    TotalRoomForSale = (availableRooms >= 0 ? availableRooms : 0) ?? 0,
                    BookedRooms = bookedRooms
                };

                Console.WriteLine($"Date: {date}, RoomType: {roomType.Name}, Total: {dailyRecord.TotalRooms}, Booked: {bookedRooms}, Available: {dailyRecord.TotalRoomForSale}, Occupancy: {dailyRecord.OccupancyPercentage:F2}%");
                return dailyRecord;
            }).ToList();

            return new RoomInventoryViewModel
            {
                RtypeID = roomType.RtypeID,
                RoomTypeName = roomType.Name,
                DailyInventory = dailyInventory
            };
        }).ToList();


        var totalOccupiedRooms = result.Sum(r => r.DailyInventory.Sum(d => d.BookedRooms));
        var totalRoomDays = result.Sum(r => r.DailyInventory.Sum(d => d.TotalRooms));
        var occupancyPercentage = totalRoomDays > 0 ?
            ((decimal)totalOccupiedRooms / totalRoomDays) * 100 : 0;

        return (result, totalRooms, occupancyPercentage);
    }

    public async Task<bool> UpdateRatesAsync(RatesUpdateViewModel model)
    {
        try
        {
            var res = await _unitOfWork.Rates.UpdateRatesAsync(model);

            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateRatesAsync)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomRatesForEnquiry(RoomRatesForEnquiry? inputDTO)
    {
        try
        {
            string sWhere = "";
            if (inputDTO != null)
            {
                if (inputDTO.PlanId != null && inputDTO.PlanId > 0)
                {
                    sWhere = (sWhere == "") ? sWhere + " r.PlanId=@PlanId " : sWhere + " and r.PlanId=@PlanId ";
                }
                if (inputDTO.RoomTypeId != null && inputDTO.RoomTypeId > 0)
                {
                    sWhere = (sWhere == "") ? sWhere + " r.RoomTypeId=@RoomTypeId " : sWhere + " and r.RoomTypeId=@RoomTypeId ";
                }
            }

            string dateFilter = "r.[Date] >= CAST(@RateDate AS DATE) AND r.[Date] < DATEADD(DAY, @NoOfNights, CAST(@RateDate AS DATE))";

            if (sWhere == "")
                sWhere = sWhere + " " + dateFilter;
            else
                sWhere = sWhere + " and " + dateFilter + " ";


            string query = @"SELECT 
                            r.Id AS RateId,
                            r.RoomTypeId AS RoomTypeId,
                            r.[Date] AS RateDate,
                            (r.Price + isnull((Select Price from Services where ID=@PlanId),0))AS Rate,
                            s.ID AS PlanId,
                            s.Service AS PlanName,
                            s.Description AS PlanDescription,
                            rt.RType RoomType,
	                        rt.Remarks RoomDescription
                        FROM Rates r
                        LEFT JOIN Services s ON r.PlanId = s.ID
                        LEFT JOIN RoomType rt ON r.RoomTypeId = rt.ID
                        WHERE " + sWhere;
            var param = new { @PlanId = inputDTO?.PlanId ?? 0, @RoomTypeId = inputDTO?.RoomTypeId ?? 0, @NoOfNights = inputDTO?.NoOfNights == 0 ? 7 : inputDTO?.NoOfNights ?? 7, @RateDate = inputDTO?.RateDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd") };
            var res = await _unitOfWork.GenOperations.GetTableData<RoomRatesForEnquiry>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateRatesAsync)}");
            throw;
        }
    }
    public async Task<IActionResult> CopyBulkRates(BulkCopyDTO inputDTO)
    {
        try
        {
            string sp = "CopyRatesFromPlan";
            var param = new DynamicParameters();
            param.Add("@FromDate", inputDTO.FromDate);
            param.Add("@ToDate", inputDTO.ToDate);
            param.Add("@FromPlanId", inputDTO.PlanIdFrom);
            param.Add("@ToPlanId", inputDTO.PlanIdTo);
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var res = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync(sp, param);            
            int result = param.Get<int>("@Status");
            if (result > 0)
            {
                return Ok("");
            }
            else
            {
                return BadRequest("");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CopyBulkRates)}");
            throw;
        }
    }
}
