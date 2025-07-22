using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.ViewModels.Guests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentAPIController> _logger;
    private readonly IMapper _mapper;
    public PaymentAPIController(IUnitOfWork unitOfWork, ILogger<PaymentAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetPaymentsWithAttr()
    {
        try
        {
            string query = @"Select 
                            p.Id
                            ,md.CustomerName GuestName
							,md.UHID
                            ,p.GuestId
                            ,p.PaymentMode
                            ,p.ReferenceNumber
                            ,p.DateOfPayment
                            ,p.Amount
                            ,p.Comments
                            ,pm.PaymentMethodName
                            ,md.CustomerName
                            ,p.AmountReceived
							,p.Differences
							,p.ApprovalDate
							,p.ApprovedBy
							,p.ApprovalComment
							,p.Status
                            ,wm.WorkerName ApprovedByName
                            from Payment p
                            Left Join PaymentMethod pm on p.PaymentMode=pm.Id
                            Join MembersDetails md on p.GuestId=md.Id
                            Left Join EHRMS.dbo.WorkerMaster wm on p.ApprovedBy=wm.WorkerID
                            where p.IsActive=1
                            order by id desc";
            var res = await _unitOfWork.GenOperations.GetTableData<PaymentWithAttr>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetPaymentsWithAttr)}");
            throw;
        }
    }

    public async Task<IActionResult> GetPaymentsWithAttrMonthYearWise(int Month, int Year)
    {
        try
        {
            string query = @"Select 
                            p.Id
                            ,md.CustomerName GuestName
							,md.UHID
                            ,p.GuestId
                            ,p.PaymentMode
                            ,p.ReferenceNumber
                            ,p.DateOfPayment
                            ,p.Amount
                            ,p.Comments
                            ,pm.PaymentMethodName
                            ,md.CustomerName
                            ,p.AmountReceived
							,p.Differences
							,p.ApprovalDate
							,p.ApprovedBy
							,p.ApprovalComment
							,p.Status
                            ,wm.WorkerName ApprovedByName
                            from Payment p
                            Left Join PaymentMethod pm on p.PaymentMode=pm.Id
                            Join MembersDetails md on p.GuestId=md.Id
                            Left Join EHRMS.dbo.WorkerMaster wm on p.ApprovedBy=wm.WorkerID
                            where p.IsActive=1 and month(p.DateOfPayment)=@Month and year(p.DateOfPayment)=@Year and md.Status=1
                            order by id desc";
            var res = await _unitOfWork.GenOperations.GetTableData<PaymentWithAttr>(query, new { @Month = Month, @Year = Year });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetPaymentsWithAttr)}");
            throw;
        }
    }

    public async Task<IActionResult> ApprovePayment(PaymentDTO inputDTO)
    {
        try
        {
            string squery = @"Select * from Payment where Id=@Id";
            var sParam = new { @Id = inputDTO.Id };
            var res = await _unitOfWork.GenOperations.GetEntityData<Payment>(squery, sParam);
            if (res != null)
            {
                res.AmountReceived = inputDTO.AmountReceived;
                res.Differences = inputDTO.Differences;
                res.ApprovalDate = inputDTO.ApprovalDate;
                res.ApprovedBy = inputDTO.ApprovedBy;
                res.ApprovalComment = inputDTO.ApprovalComment;
                res.Status = 1;
                var updated = await _unitOfWork.Payment.UpdateAsync(res);
                if (updated)
                {
                    return Ok("");
                }
            }
            return BadRequest("Unable to approve the payment right now. Try again later");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetPaymentsWithAttr)}");
            throw;
        }
    }
}
