using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class PaymentMethodAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentMethodAPIController> _logger;
    private readonly IMapper _mapper;
    public PaymentMethodAPIController(IUnitOfWork unitOfWork, ILogger<PaymentMethodAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> List()
    {
        try
        {
            string query = "Select * from PaymentMethod where IsActive=1";
            var res = await _unitOfWork.PaymentMethod.GetTableData<PaymentMethodDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
            throw;
        }
    }
    //public async Task<IActionResult> ListWithChild()
    //{
    //    try
    //    {

    //        string query = "Select tm.*,rm.RoleName from TaskMaster tm left Join EHRMS.dbo.RoleMaster rm on tm.Department=rm.RoleID where tm.IsDeleted=0";
    //        var res = await _unitOfWork.TaskMaster.GetTableData<TaskMasterWithChild>(query);
    //        return Ok(res);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Attendance {nameof(List)}");
    //        throw;
    //    }
    //}
    public async Task<IActionResult> PaymentMethodById(int Id)
    {
        try
        {
            string query = "Select * from PaymentMethod where Id=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.PaymentMethod.GetEntityData<PaymentMethodDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PaymentMethodById)}");
            throw;
        }
    }
    public async Task<IActionResult> DeletePaymentMethod(int Id)
    {
        try
        {
            string query = "Select * from PaymentMethod where Id=@Id";
            var param = new { @Id = Id };
            PaymentMethod? dto = await _unitOfWork.PaymentMethod.GetEntityData<PaymentMethod>(query, param);
            if (dto != null)
            {
                dto.IsActive = false;
                var updated = await _unitOfWork.PaymentMethod.UpdateAsync(dto);
                if (updated)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Unable to delete right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeletePaymentMethod)}");
            throw;
        }
    }

    public async Task<IActionResult> Add(PaymentMethodDTO dto)
    {
        try
        {
            string eQuery = "Select * from PaymentMethod where IsActive=1 and PaymentMethodName=@PaymentMethodName";
            var eParam = new { @IsActive = 1, @PaymentMethodName = dto.PaymentMethodName };
            var exists = await _unitOfWork.PaymentMethod.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This Method already exists");
            }
            else
            {
                dto.Id = await _unitOfWork.PaymentMethod.AddAsync(_mapper.Map<PaymentMethod>(dto));
                if (dto.Id > 0)
                {
                    return Ok(dto);
                }
                else
                {
                    return BadRequest("Unable to add right now");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(Add)}");
            throw;
        }
    }

    public async Task<IActionResult> Update(PaymentMethodDTO dto)
    {
        try
        {
            string eQuery = "Select * from PaymentMethod where IsActive=1 and PaymentMethodName=@PaymentMethodName and Id!=@Id";
            var eParam = new { @IsActive = 1, @Id = dto.Id, @PaymentMethodName = dto.PaymentMethodName };
            var exists = await _unitOfWork.PaymentMethod.IsExists(eQuery, eParam);
            if (exists)
            {
                return BadRequest("This method already exists");
            }
            else
            {
                string query = "Select * from PaymentMethod where Id=@Id";
                var param = new { @Id = dto.Id };
                PaymentMethod? paymentMethod = await _unitOfWork.PaymentMethod.GetEntityData<PaymentMethod>(query, param);
                if (paymentMethod != null)
                {
                    paymentMethod.PaymentMethodName = dto.PaymentMethodName;
                    paymentMethod.PaymentMethodCode = dto.PaymentMethodCode;


                    var updated = await _unitOfWork.PaymentMethod.UpdateAsync(paymentMethod);
                    if (updated)
                    {
                        return Ok(paymentMethod);
                    }
                }
                return BadRequest("Unable to update right now");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(Update)}");
            throw;
        }
    }
}
