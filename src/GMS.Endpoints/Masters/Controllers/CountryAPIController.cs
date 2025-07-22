using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GMS.Endpoints.Masters;

public class CountryAPIController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryAPIController> _logger;
    private readonly IMapper _mapper;
    public CountryAPIController(IUnitOfWork unitOfWork, ILogger<CountryAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> CountryList()
    {
        try
        {
            
            string query = "Select * from countries order by name asc";
            var res = await _unitOfWork.TblCountries.GetTableData<TblCountriesDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CountryList)}");
            throw;
        }
    }
    public async Task<IActionResult> StateList()
    {
        try
        {
            string query = "Select * from TblState order by State asc";
            var res = await _unitOfWork.TblState.GetTableData<TblStateDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(StateList)}");
            throw;
        }
    }
    public async Task<IActionResult> CityList()
    {
        try
        {
            string query = "Select * from tblCity order by City asc";
            var res = await _unitOfWork.tblCity.GetTableData<tblCityDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CityList)}");
            throw;
        }
    }
}
