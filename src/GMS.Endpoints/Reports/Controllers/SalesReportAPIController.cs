using AutoMapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.ViewModels.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Reports;

[Route("api/[controller]")]
[ApiController]
public class SalesReportAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SalesReportAPIController> _logger;
    private readonly IMapper _mapper;
    public SalesReportAPIController(IUnitOfWork unitOfWork, ILogger<SalesReportAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetAllSettlements(SalesReportViewModel inputDTO)
    {
        try
        {
            string query = @"DECLARE @PrevMonth INT, @PrevYear INT;
IF @Month = 1
BEGIN
    SET @PrevMonth = 12;
    SET @PrevYear = @Year - 1;
END
ELSE
BEGIN
    SET @PrevMonth = @Month - 1;
    SET @PrevYear = @Year;
END

-- Get records from Settlement for current and previous month
SELECT * 
FROM Settlement 
WHERE IsActive = 1 
  AND (
      (MONTH(CreatedDate) = @Month AND YEAR(CreatedDate) = @Year)
      OR
      (MONTH(CreatedDate) = @PrevMonth AND YEAR(CreatedDate) = @PrevYear)
  );";
            var sParam = new { @Month = inputDTO.Month, @Year = inputDTO.Year };
            var res = await _unitOfWork.GMSFinalGuest.GetTableData<SettlementDTO>(query, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllSettlements)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAllAuditRevenue(SalesReportViewModel inputDTO)
    {
        try
        {
            string query = @"DECLARE @PrevMonth INT, @PrevYear INT;
IF @Month = 1
BEGIN
    SET @PrevMonth = 12;
    SET @PrevYear = @Year - 1;
END
ELSE
BEGIN
    SET @PrevMonth = @Month - 1;
    SET @PrevYear = @Year;
END

SELECT * 
FROM AuditedRevenue 
WHERE IsActive = 1 
  AND (
      (MONTH([Date]) = @Month AND YEAR([Date]) = @Year)
      OR
      (MONTH([Date]) = @PrevMonth AND YEAR([Date]) = @PrevYear)
  );";
            var sParam = new { @Month = inputDTO.Month, @Year = inputDTO.Year };
            var res = await _unitOfWork.GMSFinalGuest.GetTableData<AuditedRevenueDTO>(query, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAllSettlements)}");
            throw;
        }
    }
}
