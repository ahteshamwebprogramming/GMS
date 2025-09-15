using AutoMapper;
using GMS.Core.Repository;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Admin.Actions;
using GMS.Infrastructure.ViewModels.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Guests;

[Route("api/[controller]")]
[ApiController]
public class AdminActionsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminActionsAPIController> _logger;
    private readonly IMapper _mapper;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;

    public AdminActionsAPIController(IUnitOfWork unitOfWork, ILogger<AdminActionsAPIController> logger, IMapper mapper, IWebHostEnvironment hostingEnv)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _hostingEnv = hostingEnv;
    }
    public async Task<IActionResult> SearchGuests(GuestsActionViewModel inputDTO)
    {
        try
        {
            string query = @"Select * from MembersDetails
where
UHID like '%'+@SearchField+'%'
OR CustomerName like '%'+@SearchField+'%'
OR MobileNo like '%'+@SearchField+'%'";
            var par = new { SearchField = inputDTO.SearchField };
            var res = await _unitOfWork.GenOperations.GetTableData<MembersDetailsDTO>(query, par);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SearchGuests)}");
            throw;
        }
    }
    public async Task<IActionResult> SearchGuestsById(GuestsActionViewModel inputDTO)
    {
        try
        {
            string query = @"Select * from MembersDetails where Id=@Id";
            var res = await _unitOfWork.GenOperations.GetTableData<MembersDetailsDTO>(query, new { @Id = inputDTO.GuestId });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SearchGuests)}");
            throw;
        }
    }
    public async Task<IActionResult> SearchRoomAllocationByGuestId(GuestsActionViewModel inputDTO)
    {
        try
        {
            string query = @"Select * from RoomAllocation where GuestId=@Id";
            var res = await _unitOfWork.GenOperations.GetTableData<RoomAllocationDTO>(query, new { @Id = inputDTO.GuestId });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SearchGuests)}");
            throw;
        }
    }
    public async Task<IActionResult> SearchBillingByGuestId(GuestsActionViewModel inputDTO)
    {
        try
        {
            string query = @"Select * from Billing where GuestId=@Id";
            var res = await _unitOfWork.GenOperations.GetTableData<BillingDTO>(query, new { @Id = inputDTO.GuestId });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SearchGuests)}");
            throw;
        }
    }
    public async Task<IActionResult> SearchPaymentByGuestId(GuestsActionViewModel inputDTO)
    {
        try
        {
            string query = @"Select * from Payment where GuestId=@Id";
            var res = await _unitOfWork.GenOperations.GetTableData<PaymentDTO>(query, new { @Id = inputDTO.GuestId });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SearchGuests)}");
            throw;
        }
    }
    public async Task<IActionResult> SearchSettlementByGuestId(GuestsActionViewModel inputDTO)
    {
        try
        {
            string query = @"Select * from Settlement where GuestId=@Id";
            var res = await _unitOfWork.GenOperations.GetTableData<SettlementDTO>(query, new { @Id = inputDTO.GuestId });
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SearchGuests)}");
            throw;
        }
    }
}
