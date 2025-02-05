using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.ViewModels.Rooms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Rooms;

[Route("api/[controller]")]
[ApiController]
public class RoomsAvailabilityAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomsAvailabilityAPIController> _logger;
    private readonly IMapper _mapper;
    public RoomsAvailabilityAPIController(IUnitOfWork unitOfWork, ILogger<RoomsAvailabilityAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetRoomsAvailabilityList(RoomsAvailabilityViewModel? dates)
    {
        try
        {
            if (dates == null)
            {
                dates = new RoomsAvailabilityViewModel();
                dates.StartDate = DateTime.Now.Date;
                dates.EndDate = DateTime.Now.Date;
            }

            //string query = "Select * from GMSFinalGuest order by id desc";
            string query1 = @"DECLARE @StartDate DATETIME = @StartDate1;
                                DECLARE @EndDate DATETIME = @EndDate1;

                                WITH AllDates AS 
                                (
                                    SELECT @StartDate AS DateValue
                                    UNION ALL
                                    SELECT DATEADD(DAY, 1, DateValue)
                                    FROM AllDates
                                    WHERE DATEADD(DAY, 1, DateValue) <= @EndDate
                                )

                                Select * 
                                ,isnull((Select Convert(nvarchar(200),isnull(md.CustomerName,'')+' ('+md.MobileNo+')') from RoomAllocation ra Left Join MembersDetails md on ra.GuestId=md.Id where d.DateValue between ra.FD and ra.TD and IsActive=1 and r.RNumber=ra.RNumber),'Available') [AvailabilityColumn]
                                from Rooms r
                                CROSS JOIN 
                                    AllDates d
                                --Where r.RNumber in ('201','202')
                                OPTION (MAXRECURSION 0);";
            string query = @"DECLARE @StartDate DATETIME = @StartDate1;
                                DECLARE @EndDate DATETIME = @EndDate1;

                                WITH AllDates AS 
                                (
                                    SELECT @StartDate AS DateValue
                                    UNION ALL
                                    SELECT DATEADD(DAY, 1, DateValue)
                                    FROM AllDates
                                    WHERE DATEADD(DAY, 1, DateValue) <= @EndDate
                                ),
                                GuestDetails AS 
                                (
                                    SELECT 
                                        ra.RNumber,
                                        CAST(d.DateValue AS DATE) AS DateValue,
                                        STUFF((
                                            SELECT ', ' + ISNULL(md.CustomerName, '') + ' (' + md.UniqueNo + ')'
                                            FROM RoomAllocation ra_inner
                                            LEFT JOIN MembersDetails md ON ra_inner.GuestId = md.Id
                                            WHERE 
                                                ra.RNumber = ra_inner.RNumber 
                                                AND CAST(d.DateValue AS DATE) BETWEEN CAST(ra_inner.FD AS DATE) AND CAST(ra_inner.TD AS DATE)
                                                AND ra_inner.IsActive = 1
                                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AllGuests,
                                        (SELECT COUNT(*)
                                         FROM RoomAllocation ra_inner
                                         WHERE 
                                             ra.RNumber = ra_inner.RNumber 
                                             AND CAST(d.DateValue AS DATE) BETWEEN CAST(ra_inner.FD AS DATE) AND CAST(ra_inner.TD AS DATE)
                                             AND ra_inner.IsActive = 1) AS GuestCount
                                    FROM 
                                        RoomAllocation ra
                                    INNER JOIN 
                                        AllDates d ON CAST(d.DateValue AS DATE) BETWEEN CAST(ra.FD AS DATE) AND CAST(ra.TD AS DATE)
                                    WHERE 
                                        ra.IsActive = 1
                                    GROUP BY 
                                        ra.RNumber, CAST(d.DateValue AS DATE)
                                )
                                SELECT 
                                    r.*,
                                    d.DateValue,
                                    CASE 
                                        WHEN gd.GuestCount IS NOT NULL THEN 
                                            CASE 
                                                WHEN gd.GuestCount = 1 THEN gd.AllGuests
                                                ELSE LEFT(gd.AllGuests, CHARINDEX(',', gd.AllGuests) - 1) + ' + ' + CAST(gd.GuestCount - 1 AS NVARCHAR)
                                            END
                                        WHEN EXISTS (SELECT 1 FROM RoomLock rl WHERE CAST(rl.FD AS DATE) <= d.DateValue AND CAST(rl.ED AS DATE) >= d.DateValue AND Status = 1 AND rl.Rooms = r.RNumber AND rl.[Type] = 'Lock') THEN 'Locked'
                                        WHEN EXISTS (SELECT 1 FROM RoomLock rl WHERE CAST(rl.FD AS DATE) <= d.DateValue AND CAST(rl.ED AS DATE) >= d.DateValue AND Status = 1 AND rl.Rooms = r.RNumber AND rl.[Type] = 'Hold') THEN 'Hold'
                                        ELSE 'Available'
                                    END AS [AvailabilityColumn]
                                FROM 
                                    Rooms r
                                CROSS JOIN 
                                    AllDates d
                                LEFT JOIN 
                                    GuestDetails gd ON gd.RNumber = r.RNumber AND gd.DateValue = d.DateValue
                                OPTION (MAXRECURSION 0);";
            var sParam = new { @StartDate1 = dates?.StartDate, @EndDate1 = dates?.EndDate };
            var res = await _unitOfWork.GMSFinalGuest.GetTableData<RoomAvailabilityDTO>(query, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomsAvailabilityList)}");
            throw;
        }
    }
}
