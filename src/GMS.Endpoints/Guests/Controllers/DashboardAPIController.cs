using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Dashboard;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.Reports;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GMS.Endpoints.Guests;

[Route("api/[controller]")]
[ApiController]
public class DashboardAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public DashboardAPIController(IUnitOfWork unitOfWork, ILogger<DashboardAPIController> logger, IMapper mapper, IWebHostEnvironment hostingEnv, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _hostingEnv = hostingEnv;
        _configuration = configuration;
    }

    private string GetEHRMSDatabaseName()
    {
        var connectionString = _configuration.GetConnectionString("EHRMSConnectionDB");
        if (string.IsNullOrEmpty(connectionString))
        {
            return "EHRMS";
        }

        var parts = connectionString.Split(';');
        foreach (var part in parts)
        {
            if (part.Trim().StartsWith("Initial Catalog", StringComparison.OrdinalIgnoreCase) ||
                part.Trim().StartsWith("Database", StringComparison.OrdinalIgnoreCase))
            {
                var dbName = part.Split('=')[1]?.Trim();
                return dbName ?? "EHRMS";
            }
        }

        return "EHRMS";
    }



    public async Task<IActionResult> GetDashboardRoomOccupancyData(DateTime? date)
    {
        try
        {
            if (date == null)
            {
                date = DateTime.Now.Date;
            }

            string query = @"WITH Dates AS (
    SELECT CAST(DATEADD(DAY, number, DATEFROMPARTS(YEAR(@Date), MONTH(@Date), 1)) AS DATE) AS TheDate
    FROM master..spt_values
    WHERE type = 'P'
      AND number < DAY(EOMONTH(@Date))
),
OccupiedRooms AS (
    SELECT 
        R.RTypeID,
        R.RNumber,
        ISNULL(A.CheckInDate, A.FD) AS StartDate,
        ISNULL(A.CheckOutDate, A.TD) AS EndDate
    FROM RoomAllocation A
    JOIN Rooms R ON A.RNumber = R.RNumber
    WHERE A.IsActive = 1
),
RoomTypeCounts AS (
    SELECT R.RTypeID, COUNT(*) AS TotalRooms
    FROM Rooms R
    WHERE R.Status = 1
    GROUP BY R.RTypeID
),
BookedCounts AS (
    SELECT 
        D.TheDate,
        O.RTypeID,
        COUNT(DISTINCT O.RNumber) AS BookedRooms
    FROM Dates D
    JOIN OccupiedRooms O
      ON D.TheDate BETWEEN CAST(O.StartDate AS DATE) AND CAST(O.EndDate AS DATE)
    GROUP BY D.TheDate, O.RTypeID
),
RoomLocks AS (
    SELECT
        D.TheDate,
        R.RTypeID,
        R.RNumber
    FROM Dates D
    JOIN Rooms R ON R.Status = 1
    JOIN RoomLock RL
      ON RL.Status = 1
     AND RL.[Type] IN ('Lock', 'Hold')
     AND (
            (RL.Rooms IS NOT NULL AND RL.Rooms = R.RNumber)
         OR (RL.Rooms IS NULL AND RL.RType = R.RTypeID)
        )
     AND (
            (RL.FD IS NULL OR CAST(RL.FD AS DATE) <= D.TheDate)
        AND (RL.ED IS NULL OR CAST(RL.ED AS DATE) >= D.TheDate)
        )
),
LockedCounts AS (
    SELECT
        TheDate,
        RTypeID,
        COUNT(DISTINCT RNumber) AS LockedRooms
    FROM RoomLocks
    GROUP BY TheDate, RTypeID
)
SELECT 
    D.TheDate,
    RTM.ID,
    RTM.RType,
    RTC.TotalRooms,
    ISNULL(B.BookedRooms, 0) AS BookedRooms,
    RTC.TotalRooms - ISNULL(B.BookedRooms, 0) AS AvailableRooms,
    CAST(
        (CAST(ISNULL(B.BookedRooms, 0) AS DECIMAL(5,2)) * 100) / 
        NULLIF(RTC.TotalRooms, 0)
    AS DECIMAL(5,2)) AS PercentOccupied
FROM Dates D
CROSS JOIN RoomTypeCounts RTC
JOIN RoomType RTM ON RTC.RTypeID = RTM.ID
LEFT JOIN BookedCounts B
    ON D.TheDate = B.TheDate AND RTC.RTypeID = B.RTypeID
LEFT JOIN LockedCounts L
    ON D.TheDate = L.TheDate AND RTC.RTypeID = L.RTypeID
ORDER BY D.TheDate, RTM.ID;
";
            var res = await _unitOfWork.GenOperations.GetTableData<RoomOccupancyData>(query, new { @Date = date });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetDashboardRoomOccupancyData)}");
            throw;
        }
    }

    public async Task<IActionResult> GetFinancialKPIData(DateTime? date)
    {
        try
        {
            if (date == null)
            {
                date = DateTime.Now.Date;
            }

            string query = @"WITH Dates AS (
    SELECT CAST(DATEADD(DAY, number, DATEFROMPARTS(YEAR(@Date), MONTH(@Date), 1)) AS DATE) AS TheDate
    FROM master..spt_values
    WHERE type = 'P' AND number < DAY(EOMONTH(@Date))
),

-- 2. For each date, get valid GroupId + RoomNumber where member was created that day
ValidGroupRoomPerDate AS (
    SELECT 
        Distinct CONVERT(DATE, md.CreationDate) AS TheDate,
        md.GroupId,
        ra.RNumber AS RoomNumber
    FROM MembersDetails md
    LEFT JOIN RoomAllocation ra ON md.Id = ra.GuestID
    WHERE md.Status = 1 AND ra.IsActive = 1
        AND MONTH(md.CreationDate) = MONTH(@Date)
        AND YEAR(md.CreationDate) = YEAR(@Date)
),

-- 3. Sales revenue only for valid GroupId + RoomNumber per day
SalesRevenueFiltered AS (
    SELECT 
        d.TheDate,
        SUM(ar.TotalDueAmount) AS SalesRevenue
    FROM Dates d
    LEFT JOIN ValidGroupRoomPerDate vr ON vr.TheDate = d.TheDate
    LEFT JOIN AuditedRevenue ar 
        ON ar.GroupId = vr.GroupId AND ar.RoomNumber = vr.RoomNumber 
        --AND CONVERT(DATE, ar.Date) = d.TheDate
        AND ar.IsActive = 1
    GROUP BY d.TheDate
),

-- 4. Payment per date
PaymentPerDate AS (
    SELECT 
        CONVERT(DATE, CreatedDate) AS TheDate,
        SUM(Amount) AS TotalPayment
    FROM Payment
    WHERE MONTH(CreatedDate) = MONTH(@Date) AND YEAR(CreatedDate) = YEAR(@Date)
    GROUP BY CONVERT(DATE, CreatedDate)
),

-- 5. Sale Stats (First, Last, High, Low) based on grouped room + group logic
RevenuePerGroupRoom AS (
    SELECT 
        CONVERT(DATE, ar.Date) AS TheDate,
        ar.GroupId,
        ar.RoomNumber,
        SUM(ar.Charges + ar.Taxes) AS TotalDueAmount
    FROM AuditedRevenue ar
    WHERE MONTH(ar.Date) = MONTH(@Date) AND YEAR(ar.Date) = YEAR(@Date) AND ar.IsActive = 1
    GROUP BY CONVERT(DATE, ar.Date), ar.GroupId, ar.RoomNumber
),
GroupRoomCreation AS (
    SELECT DISTINCT
        ar.GroupId,
        ar.RoomNumber,
        CONVERT(DATE, ar.Date) AS TheDate,
        MIN(md.CreationDate) OVER (PARTITION BY ar.GroupId, ar.RoomNumber) AS CreationDate
    FROM AuditedRevenue ar
    LEFT JOIN MembersDetails md ON md.GroupId = ar.GroupId
    LEFT JOIN RoomAllocation ra ON ra.GuestID = md.Id AND ra.RNumber = ar.RoomNumber AND ra.IsActive = 1
    WHERE MONTH(ar.Date) = MONTH(@Date) AND YEAR(ar.Date) = YEAR(@Date) AND ar.IsActive = 1
),
MergedRevenue AS (
    SELECT 
        r.TheDate,
        r.GroupId,
        r.RoomNumber,
        r.TotalDueAmount,
        g.CreationDate
    FROM RevenuePerGroupRoom r
    LEFT JOIN GroupRoomCreation g 
        ON r.GroupId = g.GroupId AND r.RoomNumber = g.RoomNumber AND r.TheDate = g.TheDate
),
RankedRevenue AS (
    SELECT *,
        ROW_NUMBER() OVER (PARTITION BY TheDate ORDER BY CreationDate ASC) AS RowNumFirst,
        ROW_NUMBER() OVER (PARTITION BY TheDate ORDER BY CreationDate DESC) AS RowNumLast,
        RANK() OVER (PARTITION BY TheDate ORDER BY TotalDueAmount DESC) AS RankHighest,
        RANK() OVER (PARTITION BY TheDate ORDER BY TotalDueAmount ASC) AS RankLowest
    FROM MergedRevenue
),
RevenueStats AS (
    SELECT 
        TheDate,
        SUM(CASE WHEN RowNumFirst = 1 THEN TotalDueAmount ELSE 0 END) AS FirstSale,
        SUM(CASE WHEN RowNumLast = 1 THEN TotalDueAmount ELSE 0 END) AS LastSale,
        SUM(CASE WHEN RankHighest = 1 THEN TotalDueAmount ELSE 0 END) AS HighestSale,
        SUM(CASE WHEN RankLowest = 1 THEN TotalDueAmount ELSE 0 END) AS LowestSale
    FROM RankedRevenue
    GROUP BY TheDate
)

-- Final Output
SELECT 
    d.TheDate,
    ISNULL(s.SalesRevenue, 0) AS SalesRevenue,
    ISNULL(p.TotalPayment, 0) AS PaymentCollected,
    ISNULL(rs.FirstSale, 0) AS FirstSale,
    ISNULL(rs.LastSale, 0) AS LastSale,
    ISNULL(rs.HighestSale, 0) AS HighestSale,
    ISNULL(rs.LowestSale, 0) AS LowestSale
FROM Dates d
LEFT JOIN SalesRevenueFiltered s ON d.TheDate = s.TheDate
LEFT JOIN PaymentPerDate p ON d.TheDate = p.TheDate
LEFT JOIN RevenueStats rs ON d.TheDate = rs.TheDate
ORDER BY d.TheDate;";
            var res = await _unitOfWork.GenOperations.GetTableData<FinancialKPIData>(query, new { @Date = date });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetDashboardRoomOccupancyData)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomRevenueData(DateTime? date)
    {
        try
        {
            if (date == null)
            {
                date = DateTime.Now.Date;
            }
            string query = @"WITH Dates AS (
                            SELECT CAST(DATEADD(DAY, number, DATEFROMPARTS(YEAR(@Date), MONTH(@Date), 1)) AS DATE) AS TheDate
                            FROM master..spt_values
                            WHERE type = 'P'
                              AND number < DAY(EOMONTH(@Date))
                        ),
                        RoomsRevenue AS (
                            SELECT 
                                d.TheDate,
                                a.Charges  -- assuming the revenue column is named ""Revenue""
                            FROM Dates d
                            LEFT JOIN AuditedRevenue a ON CONVERT(date,d.TheDate) = Convert(date,a.Date) and a.IsActive=1 and a.ChargesCategory='RoomCharges'
                        )
                        SELECT 
                            TheDate,
                            SUM(ISNULL(Charges, 0)) AS TotalRevenue
                        FROM RoomsRevenue
                        GROUP BY TheDate
                        ORDER BY TheDate;";
            var res = await _unitOfWork.GenOperations.GetTableData<RoomRevenueData>(query, new {@Date = date });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPackageRevenueData(DateTime? date)
    {
        try
        {
            if (date == null)
            {
                date = DateTime.Now.Date;
            }


            string query = @"WITH Dates AS (
                            SELECT CAST(DATEADD(DAY, number, DATEFROMPARTS(YEAR(@Date), MONTH(@Date), 1)) AS DATE) AS TheDate
                            FROM master..spt_values
                            WHERE type = 'P'
                              AND number < DAY(EOMONTH(@Date))
                        ),
                        RoomsRevenue AS (
                            SELECT 
                                d.TheDate,
                                a.Charges  -- assuming the revenue column is named ""Revenue""
                            FROM Dates d
                            LEFT JOIN AuditedRevenue a ON Convert(date,d.TheDate) = Convert(date,a.Date) and a.IsActive=1 and a.ChargesCategory in ('PackageSystem','Package')
                        )
                        SELECT 
                            TheDate,
                            SUM(ISNULL(Charges, 0)) AS TotalRevenue
                        FROM RoomsRevenue
                        GROUP BY TheDate
                        ORDER BY TheDate;";
            var res = await _unitOfWork.GenOperations.GetTableData<RevenueData>(query, new { @Date = date });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetUpsellRevenueData(DateTime? date)
    {
        try
        {
            if (date == null)
            {
                date = DateTime.Now.Date;
            }

            string query = @"WITH Dates AS (
                            SELECT CAST(DATEADD(DAY, number, DATEFROMPARTS(YEAR(@Date), MONTH(@Date), 1)) AS DATE) AS TheDate
                            FROM master..spt_values
                            WHERE type = 'P'
                              AND number < DAY(EOMONTH(@Date))
                        ),
                        RoomsRevenue AS (
                            SELECT 
                                d.TheDate,
                                a.Charges  -- assuming the revenue column is named ""Revenue""
                            FROM Dates d
                                LEFT JOIN AuditedRevenue a ON Convert(date,d.TheDate) = Convert(date,a.Date) and a.IsActive=1 and a.ChargesCategory not in ('PackageSystem','Package','RoomCharges')
                        )
                        SELECT 
                            TheDate,
                            SUM(ISNULL(Charges, 0)) AS TotalRevenue
                        FROM RoomsRevenue
                        GROUP BY TheDate
                        ORDER BY TheDate;";
            var res = await _unitOfWork.GenOperations.GetTableData<RevenueData>(query, new { @Date = date });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAverageSellingRate()
    {
        try
        {
            string query = @"SELECT 
                                AVG(Price) AS Value
                            FROM 
                                Rates
                            WHERE 
                                CAST(Date AS DATE) = CAST(GETDATE() AS DATE);";
            var res = await _unitOfWork.GenOperations.GetEntityData<Result>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAverageSellingRate_Rooms_Audit()
    {
        try
        {
            string query = @"Select avg(TotalDueAmount) Value from AuditedRevenue where ChargesCategory='RoomCharges'";
            var res = await _unitOfWork.GenOperations.GetEntityData<Result>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAverageSellingRate_Packages_Audit()
    {
        try
        {
            string query = @"Select avg(TotalDueAmount) Value from AuditedRevenue where ChargesCategory in ('PackageSystem')";
            var res = await _unitOfWork.GenOperations.GetEntityData<Result>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAverageSellingRateOverall()
    {
        try
        {
            string query = @"SELECT 
                                AVG(Price) AS Value
                            FROM 
                                Rates";
            var res = await _unitOfWork.GenOperations.GetEntityData<Result>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> AverageOccupancYearly()
    {
        try
        {
            string query = @"DECLARE @MinDate DATE = (SELECT MIN(CAST(FD AS DATE)) FROM RoomAllocation WHERE YEAR(FD) = YEAR(GETDATE()));
DECLARE @MaxDate DATE = (SELECT MAX(CAST(TD AS DATE)) FROM RoomAllocation WHERE YEAR(TD) = YEAR(GETDATE()));

;WITH Dates AS
(
    SELECT @MinDate AS DateValue
    UNION ALL
    SELECT DATEADD(DAY, 1, DateValue)
    FROM Dates
    WHERE DateValue < @MaxDate
),
DailyOccupancy AS
(
    SELECT 
        d.DateValue,
        COUNT(DISTINCT r.RNumber) AS RoomsOccupied
    FROM Dates d
    LEFT JOIN RoomAllocation r
        ON d.DateValue >= CAST(r.FD AS DATE)
       AND d.DateValue <  CAST(r.TD AS DATE)
       AND r.IsActive = 1
    GROUP BY d.DateValue
)
SELECT 
    CAST(AVG(CAST(RoomsOccupied AS DECIMAL(10,5))) AS DECIMAL(10,5)) AS Value
FROM DailyOccupancy
OPTION (MAXRECURSION 0);";
            var res = await _unitOfWork.GenOperations.GetEntityData<Result>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }

    public async Task<IActionResult> GetDashboardRoomOccupancyDataCurrentDate()
    {
        try
        {
            string query = @"WITH Dates AS (
    SELECT CAST(GETDATE() AS DATE) AS TheDate
),
OccupiedRooms AS (
    SELECT 
        R.RTypeID,
        R.RNumber,
        ISNULL(A.CheckInDate, A.FD) AS StartDate,
        ISNULL(A.CheckOutDate, A.TD) AS EndDate
    FROM RoomAllocation A
    JOIN Rooms R ON A.RNumber = R.RNumber
    WHERE A.IsActive = 1
),
RoomTypeCounts AS (
    SELECT R.RTypeID, COUNT(*) AS TotalRooms
    FROM Rooms R
    WHERE R.Status = 1
    GROUP BY R.RTypeID
),
BookedCounts AS (
    SELECT 
        D.TheDate,
        O.RTypeID,
        COUNT(DISTINCT O.RNumber) AS BookedRooms
    FROM Dates D
    JOIN OccupiedRooms O
      ON D.TheDate BETWEEN CAST(O.StartDate AS DATE) AND CAST(O.EndDate AS DATE)
    GROUP BY D.TheDate, O.RTypeID
),
TidyRooms AS (
    SELECT 
        R.RTypeID,
        COUNT(*) AS TidyRoomCount
    FROM Rooms R
    OUTER APPLY (
        SELECT TOP 1 ChkDate
        FROM RoomChkList rcl
        WHERE rcl.RID = R.ID
        ORDER BY ChkDate DESC
    ) rclLatest
    WHERE 
        R.Status = 1
        AND R.RoomClean = 1
        AND rclLatest.ChkDate IS NOT NULL
        AND DATEDIFF(HOUR, rclLatest.ChkDate, GETDATE()) <= 24
    GROUP BY R.RTypeID
),
CheckIns AS (
    SELECT 
        R.RTypeID,
        COUNT(*) AS TodayCheckIns
    FROM RoomAllocation A
    JOIN Rooms R ON A.RNumber = R.RNumber
    WHERE 
        A.IsActive = 1 AND
        CAST(A.CheckInDate AS DATE) = CAST(GETDATE() AS DATE)
    GROUP BY R.RTypeID
),
CheckOuts AS (
    SELECT 
        R.RTypeID,
        COUNT(*) AS TodayCheckOuts
    FROM RoomAllocation A
    JOIN Rooms R ON A.RNumber = R.RNumber
    WHERE 
        A.IsActive = 1 AND
        CAST(A.CheckOutDate AS DATE) = CAST(GETDATE() AS DATE)
    GROUP BY R.RTypeID
)
SELECT 
    D.TheDate,
    RTM.ID,
    RTM.RType,
    RTC.TotalRooms,
    ISNULL(B.BookedRooms, 0) AS BookedRooms,
    RTC.TotalRooms - ISNULL(B.BookedRooms, 0) AS AvailableRooms,
    ISNULL(TR.TidyRoomCount, 0) AS TidyRooms,
    ISNULL(CI.TodayCheckIns, 0) AS TodayCheckIns,
    ISNULL(CO.TodayCheckOuts, 0) AS TodayCheckOuts,
    CAST(
        (CAST(ISNULL(B.BookedRooms, 0) AS DECIMAL(5,2)) * 100) / 
        NULLIF(RTC.TotalRooms, 0)
    AS DECIMAL(5,2)) AS PercentOccupied
FROM Dates D
CROSS JOIN RoomTypeCounts RTC
JOIN RoomType RTM ON RTC.RTypeID = RTM.ID
LEFT JOIN BookedCounts B ON D.TheDate = B.TheDate AND RTC.RTypeID = B.RTypeID
LEFT JOIN TidyRooms TR ON RTC.RTypeID = TR.RTypeID
LEFT JOIN CheckIns CI ON RTC.RTypeID = CI.RTypeID
LEFT JOIN CheckOuts CO ON RTC.RTypeID = CO.RTypeID
ORDER BY D.TheDate, RTM.ID;
;
                            ";
            var res = await _unitOfWork.GenOperations.GetTableData<RoomOccupancyData>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetDashboardRoomOccupancyData)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomRevenueDataCurrentDate()
    {
        try
        {
            string query = @"WITH Dates AS (
                                SELECT CAST(GETDATE() AS DATE) AS TheDate
                            ),
                            RoomsRevenue AS (
                                SELECT 
                                    d.TheDate,
                                    a.Charges
                                FROM Dates d
                                LEFT JOIN AuditedRevenue a 
                                    ON d.TheDate = a.Date 
                                   AND a.IsActive = 1 
                                   AND a.ChargesCategory = 'RoomCharges'
                            ),
                            -- MTD: Month to Date
                            MTDRevenue AS (
                                SELECT SUM(Charges) AS MTD FROM AuditedRevenue
                                WHERE CAST(Date AS DATE) >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                  AND CAST(Date AS DATE) <= CAST(GETDATE() AS DATE)
                                  AND IsActive = 1 AND ChargesCategory = 'RoomCharges'
                            ),
                            -- YTD: Year to Date
                            YTDRevenue AS (
                                SELECT SUM(Charges) AS YTD FROM AuditedRevenue
                                WHERE CAST(Date AS DATE) >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
                                  AND CAST(Date AS DATE) <= CAST(GETDATE() AS DATE)
                                  AND IsActive = 1 AND ChargesCategory = 'RoomCharges'
                            ),
                            -- CPD: Change from Previous Day
                            PrevDay AS (
                                SELECT SUM(Charges) AS PrevDayRev FROM AuditedRevenue 
                                WHERE Date = DATEADD(DAY, -1, CAST(GETDATE() AS DATE))
                                  AND IsActive = 1 AND ChargesCategory = 'RoomCharges'
                            ),
                            -- CPM: Change from Previous Month
                            PrevMonth AS (
                                SELECT SUM(Charges) AS PrevMonthRev FROM AuditedRevenue 
                                WHERE Date >= DATEADD(MONTH, -1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
                                  AND Date < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                  AND IsActive = 1 AND ChargesCategory = 'RoomCharges'
                            ),
                            -- CPY: Change from Previous Year YTD
                            PrevYear AS (
                                SELECT SUM(Charges) AS PrevYearRev FROM AuditedRevenue 
                                WHERE Date >= DATEFROMPARTS(YEAR(GETDATE()) - 1, 1, 1) 
                                  AND Date <= DATEFROMPARTS(YEAR(GETDATE()) - 1, MONTH(GETDATE()), DAY(GETDATE()))
                                  AND IsActive = 1 AND ChargesCategory = 'RoomCharges'
                            )

                            SELECT 
                                r.TheDate,
                                ISNULL(r.Charges, 0) AS TotalRevenue,

                                ISNULL(mtd.MTD, 0) AS MTDRevenue,
                                ISNULL(ytd.YTD, 0) AS YTDRevenue,

                                (Case when ISNULL(p.PrevDayRev, 0)=0 then 0 else ((ISNULL(r.Charges, 0) - ISNULL(p.PrevDayRev, 0)) / ISNULL(p.PrevDayRev, 0)) end)  AS CPD_Change,
                                (Case when ISNULL(pm.PrevMonthRev, 0)=0 then 0 else ((ISNULL(mtd.MTD, 0) - ISNULL(pm.PrevMonthRev, 0)) / (ISNULL(pm.PrevMonthRev, 0)) ) end) AS CPM_Change,
                                (Case When ISNULL(py.PrevYearRev, 0)=0 then 0 else ((ISNULL(ytd.YTD, 0) - ISNULL(py.PrevYearRev, 0))/(ISNULL(py.PrevYearRev, 0))) end) AS CPY_Change

                            FROM RoomsRevenue r
                            CROSS JOIN MTDRevenue mtd
                            CROSS JOIN YTDRevenue ytd
                            CROSS JOIN PrevDay p
                            CROSS JOIN PrevMonth pm
                            CROSS JOIN PrevYear py;";
            var res = await _unitOfWork.GenOperations.GetTableData<RoomRevenueData>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPackagesDataCurrentDate()
    {
        try
        {
            string query = @"WITH Dates AS (
                                SELECT CAST(GETDATE() AS DATE) AS TheDate
                            ),
                            RoomsRevenue AS (
                                SELECT 
                                    d.TheDate,
                                    a.Charges
                                FROM Dates d
                                LEFT JOIN AuditedRevenue a 
                                    ON d.TheDate = a.Date 
                                   AND a.IsActive = 1 
                                   AND a.ChargesCategory in ('PackageSystem','Package')
                            ),
                            -- MTD: Month to Date
                            MTDRevenue AS (
                                SELECT SUM(Charges) AS MTD FROM AuditedRevenue
                                WHERE CAST(Date AS DATE) >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                  AND CAST(Date AS DATE) <= CAST(GETDATE() AS DATE)
                                  AND IsActive = 1 AND ChargesCategory in ('PackageSystem','Package')
                            ),
                            -- YTD: Year to Date
                            YTDRevenue AS (
                                SELECT SUM(Charges) AS YTD FROM AuditedRevenue
                                WHERE CAST(Date AS DATE) >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
                                  AND CAST(Date AS DATE) <= CAST(GETDATE() AS DATE)
                                  AND IsActive = 1 AND ChargesCategory in ('PackageSystem','Package')
                            ),
                            -- CPD: Change from Previous Day
                            PrevDay AS (
                                SELECT SUM(Charges) AS PrevDayRev FROM AuditedRevenue 
                                WHERE Date = DATEADD(DAY, -1, CAST(GETDATE() AS DATE))
                                  AND IsActive = 1 AND ChargesCategory in ('PackageSystem','Package')
                            ),
                            -- CPM: Change from Previous Month
                            PrevMonth AS (
                                SELECT SUM(Charges) AS PrevMonthRev FROM AuditedRevenue 
                                WHERE Date >= DATEADD(MONTH, -1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
                                  AND Date < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                  AND IsActive = 1 AND ChargesCategory in ('PackageSystem','Package')
                            ),
                            -- CPY: Change from Previous Year YTD
                            PrevYear AS (
                                SELECT SUM(Charges) AS PrevYearRev FROM AuditedRevenue 
                                WHERE Date >= DATEFROMPARTS(YEAR(GETDATE()) - 1, 1, 1) 
                                  AND Date <= DATEFROMPARTS(YEAR(GETDATE()) - 1, MONTH(GETDATE()), DAY(GETDATE()))
                                  AND IsActive = 1 AND ChargesCategory in ('PackageSystem','Package')
                            )

                            SELECT 
                                r.TheDate,
                                ISNULL(r.Charges, 0) AS TotalRevenue,

                                ISNULL(mtd.MTD, 0) AS MTDRevenue,
                                ISNULL(ytd.YTD, 0) AS YTDRevenue,

                                (Case when ISNULL(p.PrevDayRev, 0)=0 then 0 else ((ISNULL(r.Charges, 0) - ISNULL(p.PrevDayRev, 0)) / ISNULL(p.PrevDayRev, 0)) end)  AS CPD_Change,
                                (Case when ISNULL(pm.PrevMonthRev, 0)=0 then 0 else ((ISNULL(mtd.MTD, 0) - ISNULL(pm.PrevMonthRev, 0)) / (ISNULL(pm.PrevMonthRev, 0)) ) end) AS CPM_Change,
                                (Case When ISNULL(py.PrevYearRev, 0)=0 then 0 else ((ISNULL(ytd.YTD, 0) - ISNULL(py.PrevYearRev, 0))/(ISNULL(py.PrevYearRev, 0))) end) AS CPY_Change

                            FROM RoomsRevenue r
                            CROSS JOIN MTDRevenue mtd
                            CROSS JOIN YTDRevenue ytd
                            CROSS JOIN PrevDay p
                            CROSS JOIN PrevMonth pm
                            CROSS JOIN PrevYear py;";
            var res = await _unitOfWork.GenOperations.GetTableData<RevenueData>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetUpsellRevenueDataCurrentDate()
    {
        try
        {
            string query = @"WITH Dates AS (
                                SELECT CAST(GETDATE() AS DATE) AS TheDate
                            ),
                            RoomsRevenue AS (
                                SELECT 
                                    d.TheDate,
                                    a.Charges
                                FROM Dates d
                                LEFT JOIN AuditedRevenue a 
                                    ON d.TheDate = a.Date 
                                   AND a.IsActive = 1 
                                   AND a.ChargesCategory not in ('PackageSystem','Package','RoomCharges')
                            ),
                            -- MTD: Month to Date
                            MTDRevenue AS (
                                SELECT SUM(Charges) AS MTD FROM AuditedRevenue
                                WHERE CAST(Date AS DATE) >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                  AND CAST(Date AS DATE) <= CAST(GETDATE() AS DATE)
                                  AND IsActive = 1 AND ChargesCategory not in ('PackageSystem','Package','RoomCharges')
                            ),
                            -- YTD: Year to Date
                            YTDRevenue AS (
                                SELECT SUM(Charges) AS YTD FROM AuditedRevenue
                                WHERE CAST(Date AS DATE) >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
                                  AND CAST(Date AS DATE) <= CAST(GETDATE() AS DATE)
                                  AND IsActive = 1 AND ChargesCategory not in ('PackageSystem','Package','RoomCharges')
                            ),
                            -- CPD: Change from Previous Day
                            PrevDay AS (
                                SELECT SUM(Charges) AS PrevDayRev FROM AuditedRevenue 
                                WHERE Date = DATEADD(DAY, -1, CAST(GETDATE() AS DATE))
                                  AND IsActive = 1 AND ChargesCategory not in ('PackageSystem','Package','RoomCharges')
                            ),
                            -- CPM: Change from Previous Month
                            PrevMonth AS (
                                SELECT SUM(Charges) AS PrevMonthRev FROM AuditedRevenue 
                                WHERE Date >= DATEADD(MONTH, -1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) 
                                  AND Date < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                  AND IsActive = 1 AND ChargesCategory not in ('PackageSystem','Package','RoomCharges')
                            ),
                            -- CPY: Change from Previous Year YTD
                            PrevYear AS (
                                SELECT SUM(Charges) AS PrevYearRev FROM AuditedRevenue 
                                WHERE Date >= DATEFROMPARTS(YEAR(GETDATE()) - 1, 1, 1) 
                                  AND Date <= DATEFROMPARTS(YEAR(GETDATE()) - 1, MONTH(GETDATE()), DAY(GETDATE()))
                                  AND IsActive = 1 AND ChargesCategory not in ('PackageSystem','Package','RoomCharges')
                            )

                            SELECT 
                                r.TheDate,
                                ISNULL(r.Charges, 0) AS TotalRevenue,

                                ISNULL(mtd.MTD, 0) AS MTDRevenue,
                                ISNULL(ytd.YTD, 0) AS YTDRevenue,

                                (Case when ISNULL(p.PrevDayRev, 0)=0 then 0 else ((ISNULL(r.Charges, 0) - ISNULL(p.PrevDayRev, 0)) / ISNULL(p.PrevDayRev, 0)) end)  AS CPD_Change,
                                (Case when ISNULL(pm.PrevMonthRev, 0)=0 then 0 else ((ISNULL(mtd.MTD, 0) - ISNULL(pm.PrevMonthRev, 0)) / (ISNULL(pm.PrevMonthRev, 0)) ) end) AS CPM_Change,
                                (Case When ISNULL(py.PrevYearRev, 0)=0 then 0 else ((ISNULL(ytd.YTD, 0) - ISNULL(py.PrevYearRev, 0))/(ISNULL(py.PrevYearRev, 0))) end) AS CPY_Change

                            FROM RoomsRevenue r
                            CROSS JOIN MTDRevenue mtd
                            CROSS JOIN YTDRevenue ytd
                            CROSS JOIN PrevDay p
                            CROSS JOIN PrevMonth pm
                            CROSS JOIN PrevYear py;";
            var res = await _unitOfWork.GenOperations.GetTableData<RevenueData>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPaymentDataCurrentDate()
    {
        try
        {
            string query = @"WITH Dates AS ( SELECT CAST(GETDATE() AS DATE) AS TheDate ),
                        PaymentCollected AS (
                            SELECT 
                                d.TheDate,
                                a.Amount  -- assuming the revenue column is named ""Revenue""
                            FROM Dates d
                            LEFT JOIN Payment a ON cast(d.TheDate as date) = cast(a.DateOfPayment as date) and a.IsActive=1 
                        )
                        SELECT 
                            TheDate,
                            SUM(ISNULL(Amount, 0)) AS TotalRevenue
                        FROM PaymentCollected
                        GROUP BY TheDate
                        ORDER BY TheDate;";
            var res = await _unitOfWork.GenOperations.GetTableData<RevenueData>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }

    public async Task<IActionResult> GetADRREVPARPERIODWISE(string Period)
    {
        try
        {
            string query = @"--DECLARE @PeriodType VARCHAR(10) = 'YTD'  -- Options: TODAY,YESTERDAY, MTD, YTD, CPD, CPM, CPY
DECLARE @Today DATE = CAST(GETDATE() AS DATE)
DECLARE @StartDate DATE
DECLARE @EndDate DATE

-- Determine date range based on @PeriodType
SET @StartDate = 
    CASE 
        WHEN @PeriodType = 'TODAY' THEN @Today
        WHEN @PeriodType = 'YESTERDAY' THEN DATEADD(D,-1,@today)
        WHEN @PeriodType = 'MTD' THEN DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1)
        WHEN @PeriodType = 'YTD' THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        WHEN @PeriodType = 'CPD' THEN DATEADD(DAY, -30, @Today)  -- Last 30 days
        WHEN @PeriodType = 'CPM' THEN DATEADD(MONTH, -1, DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1)) -- Last Month
        WHEN @PeriodType = 'CPY' THEN DATEADD(YEAR, -1, DATEFROMPARTS(YEAR(@Today), 1, 1))  -- Last Year
    END

SET @EndDate = 
    CASE 
        WHEN @PeriodType IN ('TODAY') THEN @Today
        WHEN @PeriodType = 'YESTERDAY' THEN DATEADD(D,-1,@today)
        WHEN @PeriodType IN ('MTD', 'CPM') THEN EOMONTH(@StartDate)
        WHEN @PeriodType IN ('YTD', 'CPY') THEN EOMONTH(DATEADD(MONTH, 11, @StartDate))
        WHEN @PeriodType = 'CPD' THEN @Today
    END
;

WITH Dates AS (
    SELECT DATEADD(DAY, number, @StartDate) AS TheDate
    FROM master..spt_values
    WHERE type = 'P'
      AND DATEADD(DAY, number, @StartDate) <= @EndDate
),
OccupiedRooms AS (
    SELECT 
        R.RTypeID,
        R.RNumber,
        ISNULL(A.CheckInDate, A.FD) AS StartDate,
        ISNULL(A.CheckOutDate, A.TD) AS EndDate
    FROM RoomAllocation A
    JOIN Rooms R ON A.RNumber = R.RNumber
    WHERE A.IsActive = 1
),
RoomTypeCounts AS (
    SELECT R.RTypeID, COUNT(*) AS TotalRooms
    FROM Rooms R
    WHERE R.Status = 1
    GROUP BY R.RTypeID
),
BookedCounts AS (
    SELECT 
        D.TheDate,
        O.RTypeID,
        COUNT(DISTINCT O.RNumber) AS BookedRooms
    FROM Dates D
    JOIN OccupiedRooms O
      ON D.TheDate BETWEEN CAST(O.StartDate AS DATE) AND CAST(O.EndDate AS DATE)
    GROUP BY D.TheDate, O.RTypeID
),
RoomOccupancyPeriod AS (
    SELECT 
        D.TheDate,
        RTC.RTypeID,
        RTC.TotalRooms,
        ISNULL(B.BookedRooms, 0) AS BookedRooms
    FROM Dates D
    CROSS JOIN RoomTypeCounts RTC
    LEFT JOIN BookedCounts B
        ON D.TheDate = B.TheDate AND RTC.RTypeID = B.RTypeID
),
RoomsRevenue AS (
    SELECT 
        d.TheDate,
        ISNULL(a.Charges, 0) AS Revenue
    FROM Dates d
    LEFT JOIN AuditedRevenue a ON d.TheDate = a.Date AND a.IsActive = 1
),
Combined AS (
    SELECT 
        R.TheDate,
        SUM(R.BookedRooms) AS TotalBookedRooms,
        SUM(R.TotalRooms) AS TotalRooms,
        SUM(RR.Revenue) AS TotalRevenue
    FROM RoomOccupancyPeriod R
    LEFT JOIN RoomsRevenue RR ON R.TheDate = RR.TheDate
    GROUP BY R.TheDate
)
SELECT
    @PeriodType AS Period,
    SUM(TotalRevenue) AS TotalRevenue,
    SUM(TotalBookedRooms) AS TotalBookedRooms,
    SUM(TotalRooms) AS TotalRooms,
    CASE 
        WHEN SUM(TotalBookedRooms) = 0 THEN 0
        ELSE SUM(TotalRevenue) / SUM(TotalBookedRooms)
    END AS ADR,
    CASE 
        WHEN COUNT(*) * SUM(TotalRooms) = 0 THEN 0
        ELSE SUM(TotalRevenue) / (COUNT(*) * SUM(TotalRooms))  -- RevPAR = Revenue / Available Room Nights
    END AS RevPAR
FROM Combined;
";

            var param = new { @PeriodType = Period };

            var res = await _unitOfWork.GenOperations.GetEntityData<RevenueDataADRREVPARPERIODWISE>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomRevenueData)}");
            throw;
        }
    }

    [HttpGet("GetWellnessStatusBoardData")]
    public async Task<IActionResult> GetWellnessStatusBoardData(DateTime boardDate)
    {
        try
        {
            string ehrmsDbName = GetEHRMSDatabaseName();

            string query = $@"
                SELECT 
                    gs.Id AS ScheduleId,
                    ete.Id AS ExecutionId,
                    wm1.WorkerName AS TherapistName,
                    gs.EmployeeId1 AS TherapistId,
                    tm.TaskName AS TreatmentName,
                    gs.TaskId,
                    gs.StartDateTime AS ScheduledDateTime,
                    gs.EndDateTime AS ScheduledEndDateTime,
                    gs.Duration,
                    rm.ResourceName AS TreatmentRoom,
                    gs.ResourceId,
                    md.CustomerName AS GuestName,
                    gs.GuestId,
                    ra.Rnumber AS RoomNo,
                    CASE 
                        WHEN ete.ExecutionStatus = 'Completed' THEN 4
                        WHEN ete.ExecutionStatus = 'Started' OR ete.ExecutionStatus = 'InProgress' THEN 3
                        WHEN ete.ExecutionStatus = 'Pending' THEN 2
                        WHEN ete.ExecutionStatus = 'Cancelled' OR ete.ExecutionStatus = 'Missed' THEN 5
                        WHEN gs.EmployeeId1 IS NOT NULL THEN 1
                        ELSE 0
                    END AS Status,
                    ete.ActualStartTime,
                    ete.ActualEndTime,
                    ete.DelayMinutes,
                    ete.IssueNotes AS Remarks,
                    ete.IssueReportedBit AS IssueReported,
                    ete.IssueNotes
                FROM GuestSchedule gs
                LEFT JOIN TaskMaster tm ON gs.TaskId = tm.Id
                LEFT JOIN ResourceMaster rm ON gs.ResourceId = rm.Id
                LEFT JOIN [{ehrmsDbName}].dbo.WorkerMaster wm1 ON gs.EmployeeId1 = wm1.WorkerID
                LEFT JOIN MembersDetails md ON gs.GuestId = md.Id
                LEFT JOIN RoomAllocation ra ON gs.GuestId = ra.GuestID AND ra.CheckOutDate IS NULL
                LEFT JOIN EmployeeTaskExecution ete ON gs.Id = ete.GuestScheduledTaskId
                WHERE CAST(gs.StartDateTime AS DATE) = @BoardDate
                AND (gs.IsCancelled IS NULL OR gs.IsCancelled = 0)
                AND (gs.IsDeleted IS NULL OR gs.IsDeleted = 0)
                ORDER BY gs.StartDateTime ASC";

            var parameters = new { BoardDate = boardDate };
            var schedules = await _unitOfWork.GenOperations.GetTableData<GMS.Infrastructure.ViewModels.Wellness.WellnessScheduleRow>(query, parameters);

            return Ok(schedules?.ToList() ?? new List<GMS.Infrastructure.ViewModels.Wellness.WellnessScheduleRow>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching wellness status board data");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to fetch wellness status board data." });
        }
    }

}
