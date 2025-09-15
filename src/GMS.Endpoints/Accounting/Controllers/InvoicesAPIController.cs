using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Accounting;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.ViewModels.Guests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoicesAPIController> _logger;
    private readonly IMapper _mapper;
    public InvoicesAPIController(IUnitOfWork unitOfWork, ILogger<InvoicesAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetInvoicesWithAttr()
    {
        try
        {
            string query = @"WITH GuestList AS (
                            SELECT 
                                md2.GroupId,
                                raa.RNumber,
                                GuestIds = STUFF((  
                                    SELECT ' / ' + CONVERT(NVARCHAR, md3.Id)
                                    FROM MembersDetails md3
                                    LEFT JOIN RoomAllocation raa3 ON md3.Id = raa3.GuestID
                                    WHERE md3.GroupId = md2.GroupId AND raa3.RNumber = raa.RNumber
                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
                                GuestNames = STUFF((
                                    SELECT ' / ' + md3.CustomerName
                                    FROM MembersDetails md3
                                    LEFT JOIN RoomAllocation raa3 ON md3.Id = raa3.GuestID
                                    WHERE md3.GroupId = md2.GroupId AND raa3.RNumber = raa.RNumber
                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
                                UHIds = STUFF((
                                    SELECT ' / ' + md3.UHID
                                    FROM MembersDetails md3
                                    WHERE md3.GroupId = md2.GroupId
                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                            FROM MembersDetails md2
                            LEFT JOIN RoomAllocation raa ON md2.Id = raa.GuestID
	                        Where md2.Status=1 and raa.IsActive=1
                            GROUP BY md2.GroupId, raa.RNumber
                        ),
                        BillingAgg AS (
                            SELECT
                                md2.GroupId,
                                raa.RNumber,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.Price ELSE 0 END) AS GrossAmount,
                                SUM(CASE WHEN sb.ServiceType = 'RoomCharges' THEN sb.TotalAmount ELSE 0 END) AS RoomCharges,
                                SUM(CASE WHEN sb.ServiceType IN ('PackageSystem', 'Service', 'Package') THEN sb.TotalAmount ELSE 0 END) AS TreatmentCharges,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.Discount ELSE 0 END) AS Discount,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.SGST ELSE 0 END) AS SGST,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.IGST ELSE 0 END) AS IGST,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.CGST ELSE 0 END) AS CGST,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.TotalAmount ELSE 0 END) AS AmountPayable
                            FROM Billing sb
                            JOIN MembersDetails md2 ON sb.GuestId = md2.Id
                            LEFT JOIN RoomAllocation raa ON md2.Id = raa.GuestID
	                        Where sb.IsActive=1
                            GROUP BY md2.GroupId, raa.RNumber
                        ),
                        PaymentAgg AS (
                            SELECT 
                                md2.GroupId,
                                raa.RNumber,
                                SUM(p.Amount) AS AmountReceived
                            FROM Payment p
                            INNER JOIN MembersDetails md2 ON p.GuestId = md2.Id
                            INNER JOIN RoomAllocation raa ON md2.Id = raa.GuestID
	                        Where p.IsActive=1
                            GROUP BY md2.GroupId, raa.RNumber
                        )
                        SELECT 
                            s.Id,
                            isnull(ra.CheckInDate,md1.DateOfArrival) CheckInDate,
							isnull(ra.CheckOutDate,md1.DateOfDepartment) CheckOutDate,
							md1.ServiceId NoOfNights,
							CatID PackageId,
							services.Service Package,
							roomt.RType RoomType,
                            s.CreatedDate AS InvoiceDatetime,
                            ra.RNumber,
                            gl.GuestIds,
                            gl.GuestNames,
                            gl.UHIds,
                            ba.GrossAmount,
                            ba.RoomCharges,
                            ba.TreatmentCharges,
                            ba.Discount,
                            ba.SGST,
                            ba.IGST,
                            ba.CGST,
                            ba.AmountPayable,
                            ISNULL(pa.AmountReceived, 0) AS AmountReceived,
                            ISNULL(ba.AmountPayable, 0) - ISNULL(pa.AmountReceived, 0) AS Differences,
                            s.ApprovalComment,
                            s.ApprovedBy,
                            wm.WorkerName AS ApprovedByName,
                            s.ApprovedOn,
                            s.[Status],
                            s.*
                        FROM Settlement s
                        LEFT JOIN RoomAllocation ra ON s.GuestId = ra.GuestID
                        LEFT JOIN MembersDetails md1 ON s.GuestId = md1.Id
                        LEFT JOIN GuestList gl ON md1.GroupId = gl.GroupId AND ra.RNumber = gl.RNumber
                        LEFT JOIN BillingAgg ba ON md1.GroupId = ba.GroupId AND ra.RNumber = ba.RNumber
                        LEFT JOIN PaymentAgg pa ON md1.GroupId = pa.GroupId AND ra.RNumber = pa.RNumber
                        LEFT JOIN EHRMS.dbo.WorkerMaster wm ON s.ApprovedBy = wm.WorkerID
                        Left Join Services services on md1.CatID=services.ID
						LEFT JOIN RoomType roomt on md1.RoomType=roomt.ID
                        Where S.IsActive=1 and md1.Status=1;";
            var res = await _unitOfWork.GenOperations.GetTableData<InvoicingDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetInvoicesWithAttr)}");
            throw;
        }
    }

    public async Task<IActionResult> GetInvoicesWithAttrMonthYearWise(int Month, int Year)
    {
        try
        {
            string query = @"WITH GuestList AS (
                            SELECT 
                                md2.GroupId,
                                raa.RNumber,
                                GuestIds = STUFF((  
                                    SELECT ' / ' + CONVERT(NVARCHAR, md3.Id)
                                    FROM MembersDetails md3
                                    LEFT JOIN RoomAllocation raa3 ON md3.Id = raa3.GuestID
                                    WHERE md3.GroupId = md2.GroupId AND raa3.RNumber = raa.RNumber
                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
                                GuestNames = STUFF((
                                    SELECT ' / ' + md3.CustomerName
                                    FROM MembersDetails md3
                                    LEFT JOIN RoomAllocation raa3 ON md3.Id = raa3.GuestID
                                    WHERE md3.GroupId = md2.GroupId AND raa3.RNumber = raa.RNumber
                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
                                UHIds = STUFF((
                                    SELECT ' / ' + md3.UHID
                                    FROM MembersDetails md3
                                    WHERE md3.GroupId = md2.GroupId
                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
                            FROM MembersDetails md2
                            LEFT JOIN RoomAllocation raa ON md2.Id = raa.GuestID
	                        Where md2.Status=1 and raa.IsActive=1
                            GROUP BY md2.GroupId, raa.RNumber
                        ),
                        BillingAgg AS (
                            SELECT
                                md2.GroupId,
                                raa.RNumber,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.Price ELSE 0 END) AS GrossAmount,
                                SUM(CASE WHEN sb.ServiceType = 'RoomCharges' THEN sb.TotalAmount ELSE 0 END) AS RoomCharges,
                                SUM(CASE WHEN sb.ServiceType IN ('PackageSystem', 'Service', 'Package') THEN sb.TotalAmount ELSE 0 END) AS TreatmentCharges,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.Discount ELSE 0 END) AS Discount,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.SGST ELSE 0 END) AS SGST,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.IGST ELSE 0 END) AS IGST,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.CGST ELSE 0 END) AS CGST,
                                SUM(CASE WHEN sb.ServiceType = 'GrossAmount' THEN sb.TotalAmount ELSE 0 END) AS AmountPayable
                            FROM Billing sb
                            JOIN MembersDetails md2 ON sb.GuestId = md2.Id
                            LEFT JOIN RoomAllocation raa ON md2.Id = raa.GuestID
	                        Where sb.IsActive=1
                            GROUP BY md2.GroupId, raa.RNumber
                        ),
                        PaymentAgg AS (
                            SELECT 
                                md2.GroupId,
                                raa.RNumber,
                                SUM(p.Amount) AS AmountReceived
                            FROM Payment p
                            INNER JOIN MembersDetails md2 ON p.GuestId = md2.Id
                            INNER JOIN RoomAllocation raa ON md2.Id = raa.GuestID
	                        Where p.IsActive=1
                            GROUP BY md2.GroupId, raa.RNumber
                        )
                        SELECT 
                            s.Id,
                            isnull(ra.CheckInDate,md1.DateOfArrival) CheckInDate,
							isnull(ra.CheckOutDate,md1.DateOfDepartment) CheckOutDate,
							md1.ServiceId NoOfNights,
							CatID PackageId,
							services.Service Package,
							roomt.RType RoomType,
                            s.CreatedDate AS InvoiceDatetime,
                            ra.RNumber,
                            gl.GuestIds,
                            gl.GuestNames,
                            gl.UHIds,
                            ba.GrossAmount,
                            ba.RoomCharges,
                            ba.TreatmentCharges,
                            ba.Discount,
                            ba.SGST,
                            ba.IGST,
                            ba.CGST,
                            ba.AmountPayable,
                            ISNULL(pa.AmountReceived, 0) AS AmountReceived,
                            ISNULL(ba.AmountPayable, 0) - ISNULL(pa.AmountReceived, 0) AS Differences,
                            s.ApprovalComment,
                            s.ApprovedBy,
                            wm.WorkerName AS ApprovedByName,
                            s.ApprovedOn,
                            s.[Status],
                            s.*
                        FROM Settlement s
                        LEFT JOIN RoomAllocation ra ON s.GuestId = ra.GuestID
                        LEFT JOIN MembersDetails md1 ON s.GuestId = md1.Id
                        LEFT JOIN GuestList gl ON md1.GroupId = gl.GroupId AND ra.RNumber = gl.RNumber
                        LEFT JOIN BillingAgg ba ON md1.GroupId = ba.GroupId AND ra.RNumber = ba.RNumber
                        LEFT JOIN PaymentAgg pa ON md1.GroupId = pa.GroupId AND ra.RNumber = pa.RNumber
                        LEFT JOIN EHRMS.dbo.WorkerMaster wm ON s.ApprovedBy = wm.WorkerID
                        Left Join Services services on md1.CatID=services.ID
						LEFT JOIN RoomType roomt on md1.RoomType=roomt.ID
                        Where S.IsActive=1 and md1.Status=1 and ra.IsActive=1 and Month(s.CreatedDate)=@Month and Year(s.CreatedDate)=@Year;";
            var res = await _unitOfWork.GenOperations.GetTableData<InvoicingDTO>(query,new { @Month = Month,@Year=Year });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetInvoicesWithAttr)}");
            throw;
        }
    }

    public async Task<IActionResult> ApproveInvoices(SettlementDTO inputDTO)
    {
        try
        {
            string squery = @"Select * from Settlement where Id=@Id";
            var sParam = new { @Id = inputDTO.Id };
            var res = await _unitOfWork.GenOperations.GetEntityData<Settlement>(squery, sParam);
            if (res != null)
            {
                res.ApprovalComment = inputDTO.ApprovalComment;
                res.ApprovedBy = inputDTO.ApprovedBy;
                res.ApprovedOn = inputDTO.ApprovedOn;
                res.Status = 1;
                var updated = await _unitOfWork.Settlement.UpdateAsync(res);
                if (updated)
                {
                    return Ok("");
                }
            }
            return BadRequest("Unable to approve the payment right now. Try again later");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ApproveInvoices)}");
            throw;
        }
    }
}
