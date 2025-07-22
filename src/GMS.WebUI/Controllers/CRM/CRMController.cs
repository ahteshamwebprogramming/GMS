using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.AppParameters;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.CRM;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.Rooms;
using GMS.WebUI.Controllers.Guests;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rooms.Controllers;
using System.Security.Claims;
namespace GMS.WebUI.Controllers.CRM;

public class CRMController : Controller
{
    private readonly ILogger<CRMController> _logger;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly AccountsAPIController _accountsAPIController;
    private readonly GenderAPIController _genderAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly ServicesAPIController _servicesAPIController;
    private readonly MSTCategoriesAPIController _mSTCategoriesAPIController;
    private readonly RoomTypeAPIController _roomTypeAPIController;
    private readonly CarTypeAPIController _carTypeAPIController;
    private readonly BrandAwarenessAPIController _brandAwarenessAPIController;
    private readonly LeadSourceAPIController _leadSourceAPIController;
    private readonly ChannelCodeAPIController _channelCodeAPIController;
    private readonly GuaranteeCodeAPIController _guaranteeCodeAPIController;
    private readonly RoomRateAPIController _roomRateAPIController;
    private readonly SourcesAPIController _sourcesAPIController;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    private readonly ClientInfo _clientInfo;

    public CRMController(ILogger<CRMController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, GenderAPIController genderAPIController, CountryAPIController countryAPIController, ServicesAPIController servicesAPIController, MSTCategoriesAPIController mSTCategoriesAPIController, RoomTypeAPIController roomTypeAPIController, CarTypeAPIController carTypeAPIController, BrandAwarenessAPIController brandAwarenessAPIController, LeadSourceAPIController leadSourceAPIController, ChannelCodeAPIController channelCodeAPIController, GuaranteeCodeAPIController guaranteeCodeAPIController, IWebHostEnvironment hostingEnv, IOptions<ClientInfo> clientInfoOptions, RoomRateAPIController roomRateAPIController, SourcesAPIController sourcesAPIController)
    {
        _logger = logger;
        _guestsAPIController = guestsAPIController;
        _accountsAPIController = accountsAPIController;
        _genderAPIController = genderAPIController;
        _countryAPIController = countryAPIController;
        _servicesAPIController = servicesAPIController;
        _mSTCategoriesAPIController = mSTCategoriesAPIController;
        _roomTypeAPIController = roomTypeAPIController;
        _carTypeAPIController = carTypeAPIController;
        _brandAwarenessAPIController = brandAwarenessAPIController;
        _leadSourceAPIController = leadSourceAPIController;
        _channelCodeAPIController = channelCodeAPIController;
        _guaranteeCodeAPIController = guaranteeCodeAPIController;
        _hostingEnv = hostingEnv;
        _clientInfo = clientInfoOptions.Value;
        _roomRateAPIController = roomRateAPIController;
        _sourcesAPIController = sourcesAPIController;
    }

    public IActionResult NewEnquiry()
    {
        return View();
    }
    public async Task<IActionResult> CallingList()
    {
        EnquiryListViewModel dto = new EnquiryListViewModel();

        var guestRes = await _guestsAPIController.GetEnquiryList(2);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.GuestEnquiryWithAttrList = (List<GuestReservationWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }
        return View(dto);
    }
    public IActionResult BookingList()
    {
        return View();
    }
    public async Task<IActionResult> ClosedEnquiry()
    {
        EnquiryListViewModel dto = new EnquiryListViewModel();

        var guestRes = await _guestsAPIController.GetEnquiryList(1);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.GuestEnquiryWithAttrList = (List<GuestReservationWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }
        return View(dto);

    }

    public async Task<IActionResult> EnquiryList()
    {
        EnquiryListViewModel dto = new EnquiryListViewModel();

        var guestRes = await _guestsAPIController.GetEnquiryList(3);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.GuestEnquiryWithAttrList = (List<GuestReservationWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }
        return View(dto);
    }

    public async Task<IActionResult> AddGuestsPartialViewCRM([FromBody] MembersDetailsDTO inputDTO)
    {
        GuestsListViewModel viewModel = await ViewModelAddGuestsPartial(inputDTO);
        return PartialView("_newEnquiry/_addGuests", viewModel);
    }
    public async Task<IActionResult> AddGuestsPartialView([FromBody] MembersDetailsDTO inputDTO)
    {
        GuestsListViewModel viewModel = await ViewModelAddGuestsPartial(inputDTO);
        return PartialView("_guestsList/_addGuests", viewModel);
    }

    public async Task<GuestsListViewModel> ViewModelAddGuestsPartial(MembersDetailsDTO inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();

        if (!String.IsNullOrEmpty(inputDTO.GroupId))
        {
            var guestRes = await _guestsAPIController.GetGuestByGroupIdAndPax(inputDTO.GroupId, inputDTO.PAXSno);
            if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
            {
                viewModel.MemberDetail = (MembersDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
            }
            var guestattachments = await _guestsAPIController.GetGuestAttachmentsByGroupIdAndPax(inputDTO.GroupId, inputDTO.PAXSno);
            if (guestattachments != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestattachments).StatusCode == 200)
            //if (guestattachments != null && ((Microsoft.AspNetCore.Mvc.StatusCodeResult)guestattachments).StatusCode == 200)
            {
                viewModel.GuestAttachments = (List<GuestDocumentAttachmentsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestattachments).Value;
                if (viewModel.GuestAttachments != null)
                {
                    foreach (var item in viewModel.GuestAttachments)
                    {
                        item.eId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                    }
                }
            }

        }

        if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.GroupId))
        {
            var guestRes = await _guestsAPIController.GetGuestStayInformationByGroupId(inputDTO.GroupId ?? "");
            if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
            {
                var stayInformation = (MembersDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
                if (viewModel.MemberDetail != null && stayInformation != null)
                {
                    viewModel.MemberDetail.CatId = stayInformation.CatId;
                    viewModel.MemberDetail.ServiceId = stayInformation.ServiceId;
                    viewModel.MemberDetail.AdditionalNights = stayInformation.AdditionalNights;
                    viewModel.MemberDetail.DateOfArrival = stayInformation.DateOfArrival;
                    viewModel.MemberDetail.DateOfDepartment = stayInformation.DateOfDepartment;
                    viewModel.MemberDetail.RoomType = stayInformation.RoomType;
                    viewModel.MemberDetail.Pax = stayInformation.Pax;
                    viewModel.MemberDetail.NoOfRooms = stayInformation.NoOfRooms;
                    viewModel.MemberDetail.PickUpDrop = stayInformation.PickUpDrop;
                    viewModel.MemberDetail.PickUpType = stayInformation.PickUpType;
                    viewModel.MemberDetail.CarType = stayInformation.CarType;
                    viewModel.MemberDetail.PickUpLoaction = stayInformation.PickUpLoaction;
                    viewModel.MemberDetail.FlightArrivalDateAndDateTime = stayInformation.FlightArrivalDateAndDateTime;
                    viewModel.MemberDetail.FlightDepartureDateAndDateTime = stayInformation.FlightDepartureDateAndDateTime;
                }
            }
        }

        var genderRes = await _genderAPIController.GenderList();
        if (genderRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)genderRes).StatusCode == 200)
        {
            viewModel.Genders = (List<GenderMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)genderRes).Value;
        }
        var countryRes = await _countryAPIController.CountryList();
        if (countryRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)countryRes).StatusCode == 200)
        {
            viewModel.Countries = (List<TblCountriesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)countryRes).Value;
        }
        var stateRes = await _countryAPIController.StateList();
        if (stateRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)stateRes).StatusCode == 200)
        {
            viewModel.States = (List<TblStateDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)stateRes).Value;
        }
        var cityRes = await _countryAPIController.CityList();
        if (cityRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)cityRes).StatusCode == 200)
        {
            viewModel.Cities = (List<tblCityDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)cityRes).Value;
        }
        var servicesRes = await _servicesAPIController.List();
        if (servicesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).StatusCode == 200)
        {
            viewModel.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).Value;
        }
        var roomTypes = await _roomTypeAPIController.List();
        if (roomTypes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).StatusCode == 200)
        {
            viewModel.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).Value;
        }
        var carTypes = await _carTypeAPIController.CarTypeList();
        if (carTypes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)carTypes).StatusCode == 200)
        {
            viewModel.CarTypes = (List<CarTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)carTypes).Value;
        }
        var brandAwarenesses = await _brandAwarenessAPIController.BrandAwarenessList();
        if (brandAwarenesses != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)brandAwarenesses).StatusCode == 200)
        {
            viewModel.BrandAwarenesses = (List<BrandAwarenessDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)brandAwarenesses).Value;
        }
        var leadeSources = await _leadSourceAPIController.LeadSourceList();
        if (leadeSources != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)leadeSources).StatusCode == 200)
        {
            viewModel.LeadSources = (List<LeadSourceDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)leadeSources).Value;
        }
        var channelCodes = await _channelCodeAPIController.List();
        if (channelCodes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)channelCodes).StatusCode == 200)
        {
            viewModel.ChannelCodes = (List<ChannelCodeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)channelCodes).Value;
        }
        var guarenteeCodes = await _guaranteeCodeAPIController.List();
        if (guarenteeCodes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guarenteeCodes).StatusCode == 200)
        {
            viewModel.GuaranteeCodes = (List<GuaranteeCodeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guarenteeCodes).Value;
        }
        var ratesForEnquiryRes = await _roomRateAPIController.GetRoomRatesForEnquiry(null);
        if (ratesForEnquiryRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)ratesForEnquiryRes).StatusCode == 200)
        {
            viewModel.RoomRatesForEnquiryList = (List<RoomRatesForEnquiry>?)((Microsoft.AspNetCore.Mvc.ObjectResult)ratesForEnquiryRes).Value;
        }
        var sources = await _sourcesAPIController.List();
        if (sources != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)sources).StatusCode == 200)
        {
            viewModel.SourcesList = (List<SourcesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)sources).Value;
        }
        return viewModel;
    }

    public async Task<IActionResult> RoomRatesForEnquiryCRM([FromBody] RoomRatesForEnquiry inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();
        var ratesForEnquiryRes = await _roomRateAPIController.GetRoomRatesForEnquiry(inputDTO);
        if (ratesForEnquiryRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)ratesForEnquiryRes).StatusCode == 200)
        {
            viewModel.RoomRatesForEnquiryList = (List<RoomRatesForEnquiry>?)((Microsoft.AspNetCore.Mvc.ObjectResult)ratesForEnquiryRes).Value;
        }
        return PartialView("_newEnquiry/_ratePlan", viewModel);
    }

    public async Task<IActionResult> RoomRatesForEnquiry([FromBody] RoomRatesForEnquiry inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();
        viewModel.NoOfRooms = (inputDTO.NoOfRooms == 0 ? 1 : inputDTO.NoOfRooms) ?? 1;
        var ratesForEnquiryRes = await _roomRateAPIController.GetRoomRatesForEnquiry(inputDTO);
        if (ratesForEnquiryRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)ratesForEnquiryRes).StatusCode == 200)
        {
            viewModel.RoomRatesForEnquiryList = (List<RoomRatesForEnquiry>?)((Microsoft.AspNetCore.Mvc.ObjectResult)ratesForEnquiryRes).Value;
        }
        return PartialView("_guestsList/_ratePlan", viewModel);
    }
    public async Task<IActionResult> SearchGuestDetailsByPhoneNumber([FromBody] MembersDetailsDTO inputDTO)
    {
        List<MemberDetailsWithChild>? memberDetails = new List<MemberDetailsWithChild>();
        var res = await _guestsAPIController.GetGuestDetailsByPhoneNumber(inputDTO);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            memberDetails = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_newEnquiry/_guestsSearch", memberDetails);
    }
    public async Task<IActionResult> GuestDetailsByIdForGuestFormDetails([FromBody] MembersDetailsDTO inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();

        if (inputDTO != null && inputDTO.Id > 0)
        {
            var guestRes = await _guestsAPIController.GetGuestById(inputDTO.Id);
            if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
            {
                viewModel.MemberDetail = (MembersDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;

                if (viewModel.MemberDetail != null)
                {
                    viewModel.MemberDetail.CatId = null;
                    viewModel.MemberDetail.ServiceId = null;
                    viewModel.MemberDetail.AdditionalNights = null;
                    viewModel.MemberDetail.DateOfArrival = null;
                    viewModel.MemberDetail.DateOfDepartment = null;
                    viewModel.MemberDetail.RoomType = null;
                    viewModel.MemberDetail.Pax = null;
                    viewModel.MemberDetail.PAXSno = 1;
                    viewModel.MemberDetail.NoOfRooms = null;
                    viewModel.MemberDetail.PickUpDrop = null;
                    viewModel.MemberDetail.PickUpType = null;
                    viewModel.MemberDetail.CarType = null;
                    viewModel.MemberDetail.PickUpLoaction = null;
                    viewModel.MemberDetail.FlightArrivalDateAndDateTime = null;
                    viewModel.MemberDetail.FlightDepartureDateAndDateTime = null;
                }

            }

            var guestattachments = await _guestsAPIController.GetGuestAttachmentsByGuestId(inputDTO.Id);
            if (guestattachments != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestattachments).StatusCode == 200)
            {
                viewModel.GuestAttachments = (List<GuestDocumentAttachmentsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestattachments).Value;
                if (viewModel.GuestAttachments != null)
                {
                    foreach (var item in viewModel.GuestAttachments)
                    {
                        item.eId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                    }
                }
            }

        }
        if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.GroupId))
        {
            var guestRes = await _guestsAPIController.GetGuestStayInformationByGroupId(inputDTO.GroupId ?? "");
            if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
            {
                var stayInformation = (MembersDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
                if (viewModel.MemberDetail != null && stayInformation != null)
                {
                    viewModel.MemberDetail.CatId = stayInformation.CatId;
                    viewModel.MemberDetail.ServiceId = stayInformation.ServiceId;
                    viewModel.MemberDetail.AdditionalNights = stayInformation.AdditionalNights;
                    viewModel.MemberDetail.DateOfArrival = stayInformation.DateOfArrival;
                    viewModel.MemberDetail.DateOfDepartment = stayInformation.DateOfDepartment;
                    viewModel.MemberDetail.RoomType = stayInformation.RoomType;
                    viewModel.MemberDetail.Pax = stayInformation.Pax;
                    viewModel.MemberDetail.NoOfRooms = stayInformation.NoOfRooms;
                    viewModel.MemberDetail.PickUpDrop = stayInformation.PickUpDrop;
                    viewModel.MemberDetail.PickUpType = stayInformation.PickUpType;
                    viewModel.MemberDetail.CarType = stayInformation.CarType;
                    viewModel.MemberDetail.PickUpLoaction = stayInformation.PickUpLoaction;
                    viewModel.MemberDetail.FlightArrivalDateAndDateTime = stayInformation.FlightArrivalDateAndDateTime;
                    viewModel.MemberDetail.FlightDepartureDateAndDateTime = stayInformation.FlightDepartureDateAndDateTime;
                }
            }
        }
        var genderRes = await _genderAPIController.GenderList();
        if (genderRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)genderRes).StatusCode == 200)
        {
            viewModel.Genders = (List<GenderMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)genderRes).Value;
        }
        var countryRes = await _countryAPIController.CountryList();
        if (countryRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)countryRes).StatusCode == 200)
        {
            viewModel.Countries = (List<TblCountriesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)countryRes).Value;
        }
        var stateRes = await _countryAPIController.StateList();
        if (stateRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)stateRes).StatusCode == 200)
        {
            viewModel.States = (List<TblStateDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)stateRes).Value;
        }
        var cityRes = await _countryAPIController.CityList();
        if (cityRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)cityRes).StatusCode == 200)
        {
            viewModel.Cities = (List<tblCityDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)cityRes).Value;
        }
        var servicesRes = await _servicesAPIController.List();
        if (servicesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).StatusCode == 200)
        {
            viewModel.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).Value;
        }
        var roomTypes = await _roomTypeAPIController.List();
        if (roomTypes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).StatusCode == 200)
        {
            viewModel.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).Value;
        }
        var carTypes = await _carTypeAPIController.CarTypeList();
        if (carTypes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)carTypes).StatusCode == 200)
        {
            viewModel.CarTypes = (List<CarTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)carTypes).Value;
        }
        var brandAwarenesses = await _brandAwarenessAPIController.BrandAwarenessList();
        if (brandAwarenesses != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)brandAwarenesses).StatusCode == 200)
        {
            viewModel.BrandAwarenesses = (List<BrandAwarenessDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)brandAwarenesses).Value;
        }
        var leadeSources = await _leadSourceAPIController.LeadSourceList();
        if (leadeSources != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)leadeSources).StatusCode == 200)
        {
            viewModel.LeadSources = (List<LeadSourceDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)leadeSources).Value;
        }
        var channelCodes = await _channelCodeAPIController.List();
        if (channelCodes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)channelCodes).StatusCode == 200)
        {
            viewModel.ChannelCodes = (List<ChannelCodeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)channelCodes).Value;
        }
        var guarenteeCodes = await _guaranteeCodeAPIController.List();
        if (guarenteeCodes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guarenteeCodes).StatusCode == 200)
        {
            viewModel.GuaranteeCodes = (List<GuaranteeCodeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guarenteeCodes).Value;
        }
        var sources = await _sourcesAPIController.List();
        if (sources != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)sources).StatusCode == 200)
        {
            viewModel.SourcesList = (List<SourcesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)sources).Value;
        }
        //return PartialView("_newEnquiry/_guestsFormDetails", viewModel);
        return PartialView("_newEnquiry/_guestsHotelFormDetails", viewModel);

    }
    [HttpPost]
    public async Task<IActionResult> SaveMemberEnquiryDetails(GuestsListViewModel dataVM)
    {
        try
        {
            if (dataVM != null && dataVM.MemberDetail != null)
            {
                #region CapsulateData
                GuestReservationDTO inputDTO = new GuestReservationDTO();

                inputDTO.Fname = dataVM.MemberDetail.Fname;
                inputDTO.Mname = dataVM.MemberDetail.Mname;
                inputDTO.Lname = dataVM.MemberDetail.Lname;
                inputDTO.MobileNo = dataVM.MemberDetail.MobileNo;
                inputDTO.EmailId = dataVM.MemberDetail.Email;
                inputDTO.CatId = dataVM.MemberDetail.CatId;
                inputDTO.DateOfArrival = dataVM.MemberDetail.DateOfArrival;
                inputDTO.DateOfDepartment = dataVM.MemberDetail.DateOfDepartment;
                inputDTO.Pax = dataVM.MemberDetail.Pax;
                inputDTO.RoomType = dataVM.MemberDetail.RoomType;
                inputDTO.ScheduleType = dataVM.MemberDetail.ScheduleType;
                inputDTO.ServiceId = string.IsNullOrWhiteSpace(dataVM?.MemberDetail?.ServiceId) ? 0 : Convert.ToInt32(dataVM.MemberDetail.ServiceId);

                inputDTO.Remarks = dataVM.MemberDetail.Remarks;

                inputDTO.UserId = Convert.ToInt32(User.FindFirstValue("Id"));


                inputDTO.NoOfRooms = dataVM.MemberDetail.NoOfRooms;

                inputDTO.SaleSource = dataVM.MemberDetail.SaleSource;
                inputDTO.LeadSource = dataVM.MemberDetail.LeadSource;
                inputDTO.BrandAwn = dataVM.MemberDetail.AboutUs;
                inputDTO.Status = 1;

                #endregion
                var res = await _guestsAPIController.SaveDetailsToEnquiry(inputDTO);

                return res;

            }
            else
            {
                return BadRequest("Data is invalid");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
    [HttpPost]
    public async Task<IActionResult> SaveMemberDetailsCRM(GuestsListViewModel dataVM)
    {
        try
        {
            if (dataVM != null && dataVM.MemberDetail != null)
            {
                #region CapsulateData
                MembersDetailsDTO inputDTO = new MembersDetailsDTO();
                inputDTO.Age = 0;
                inputDTO.ServiceId = dataVM.MemberDetail.ServiceId;
                inputDTO.CatId = dataVM.MemberDetail.CatId;
                inputDTO.Fname = dataVM.MemberDetail.Fname;
                inputDTO.Mname = dataVM.MemberDetail.Mname;
                inputDTO.Lname = dataVM.MemberDetail.Lname;
                inputDTO.PhNo = dataVM.MemberDetail.PhNo;
                inputDTO.MarStatus = dataVM.MemberDetail.MarStatus;
                inputDTO.Gender = dataVM.MemberDetail.Gender;
                inputDTO.EmergencyNo = dataVM.MemberDetail.EmergencyNo;
                inputDTO.Dob = dataVM.MemberDetail.Dob;
                inputDTO.AboutUs = dataVM.MemberDetail.AboutUs;
                inputDTO.Address1 = dataVM.MemberDetail.Address1;
                inputDTO.Address2 = dataVM.MemberDetail.Address2;
                inputDTO.CityId = dataVM.MemberDetail.CityId;
                inputDTO.StateId = dataVM.MemberDetail.StateId;
                inputDTO.CountryId = dataVM.MemberDetail.CountryId;
                inputDTO.Pincode = dataVM.MemberDetail.Pincode;
                inputDTO.RelativeName = dataVM.MemberDetail.RelativeName;
                inputDTO.RelativeNumber = dataVM.MemberDetail.RelativeNumber;
                inputDTO.Relations = dataVM.MemberDetail.Relations;
                inputDTO.ReferContact = dataVM.MemberDetail.ReferContact;
                inputDTO.Email = dataVM.MemberDetail.Email;
                inputDTO.MobileNo = dataVM.MemberDetail.MobileNo;
                inputDTO.Remarks = dataVM.MemberDetail.Remarks;
                inputDTO.UserId = Convert.ToInt32(User.FindFirstValue("Id"));
                inputDTO.GuarenteeCode = dataVM.MemberDetail.GuarenteeCode;
                inputDTO.PaymentStatus = dataVM.MemberDetail.PaymentStatus;
                inputDTO.LeadSource = dataVM.MemberDetail.LeadSource;
                inputDTO.ChannelCode = dataVM.MemberDetail.ChannelCode;
                inputDTO.AdditionalNights = dataVM.MemberDetail.AdditionalNights;
                inputDTO.DateOfArrival = dataVM.MemberDetail.DateOfArrival;
                inputDTO.DateOfDepartment = dataVM.MemberDetail.DateOfDepartment;
                inputDTO.Pax = dataVM.MemberDetail.Pax;
                inputDTO.NoOfRooms = dataVM.MemberDetail.NoOfRooms;

                inputDTO.Occupation = dataVM.MemberDetail.Occupation;

                inputDTO.PolicyHolder = dataVM.MemberDetail.PolicyHolder;
                inputDTO.InsuranceCompany = dataVM.MemberDetail.InsuranceCompany;
                inputDTO.PolicyNo = dataVM.MemberDetail.PolicyNo;

                inputDTO.RoomType = dataVM.MemberDetail.RoomType;
                inputDTO.HealthIssues = dataVM.MemberDetail.HealthIssues;

                inputDTO.IsMonthlynewsletter = dataVM.MemberDetail.IsMonthlynewsletter;

                inputDTO.BloodGroup = dataVM.MemberDetail.BloodGroup;

                inputDTO.CoveredIndia = dataVM.MemberDetail.CoveredIndia;
                inputDTO.Staydays = 0;

                inputDTO.Nights = "";
                inputDTO.NationalityId = dataVM.MemberDetail.NationalityId;
                inputDTO.HoldTillDate = dataVM.MemberDetail.HoldTillDate;
                inputDTO.PaymentDate = dataVM.MemberDetail.PaymentDate;
                inputDTO.PickUpDrop = dataVM.MemberDetail.PickUpDrop;
                inputDTO.PickUpType = dataVM.MemberDetail.PickUpType;
                inputDTO.CarType = dataVM.MemberDetail.CarType;
                inputDTO.CarType = dataVM.MemberDetail.CarType;
                inputDTO.FlightArrivalDateAndDateTime = dataVM.MemberDetail.FlightArrivalDateAndDateTime;
                inputDTO.FlightDepartureDateAndDateTime = dataVM.MemberDetail.FlightDepartureDateAndDateTime;
                inputDTO.PickUpLoaction = dataVM.MemberDetail.PickUpLoaction;
                inputDTO.PAXSno = dataVM.MemberDetail.PAXSno;

                inputDTO.PhotoShared = dataVM.MemberDetail.PhotoShared;
                inputDTO.Idproof = dataVM.MemberDetail.Idproof;
                inputDTO.IdProofIssueDate = dataVM.MemberDetail.IdProofIssueDate;
                inputDTO.IdProofExpiryDate = dataVM.MemberDetail.IdProofExpiryDate;
                inputDTO.PassportNo = dataVM.MemberDetail.PassportNo;
                inputDTO.PassportIssueDate = dataVM.MemberDetail.PassportIssueDate;
                inputDTO.PassportExpiryDate = dataVM.MemberDetail.PassportExpiryDate;
                inputDTO.VisaDetails = dataVM.MemberDetail.VisaDetails;
                inputDTO.VisaIssueDate = dataVM.MemberDetail.VisaIssueDate;
                inputDTO.VisaExpiryDate = dataVM.MemberDetail.VisaExpiryDate;

                inputDTO.RegistrationNumber = await _guestsAPIController.GetRegistrationNo(dataVM.MemberDetail);

                //if (dataVM.Source == "RoomAvailability")
                //{
                //    inputDTO.RoomType = dataVM.MemberDetail.RoomType;
                //    inputDTO.Pax = dataVM.MemberDetail.Pax;
                //    inputDTO.NoOfRooms = dataVM.MemberDetail.NoOfRooms;
                //    inputDTO.PAXSno = dataVM.MemberDetail.PAXSno;
                //}

                if (String.IsNullOrEmpty(dataVM.MemberDetail.GroupId))
                {
                    if (dataVM.Source == "RoomAvailability")
                    {
                        inputDTO.RoomType = await _guestsAPIController.GetRoomTypeByRoomNumber(dataVM?.RoomAllocation?.Rnumber ?? "");
                        inputDTO.Pax = 1;
                        inputDTO.NoOfRooms = 1;
                        inputDTO.PAXSno = 1;

                        //When Id = 0
                        inputDTO.IsCrm = 0; //when id=0
                        inputDTO.Status = 1; //when id=0
                        inputDTO.CreationDate = DateTime.Now; //when id=0
                        inputDTO.AprovedDate = DateTime.Now; //when id=0
                        inputDTO.IsApproved = 1;//When Id=0
                        inputDTO.ApprovedBy = Convert.ToInt32(User.FindFirstValue("Id")); //when id=0
                        inputDTO.UniqueNo = "NAAD00" + CommonHelper.GenerateNaadRandomNo();//When Id=0
                        inputDTO.GroupId = await _guestsAPIController.GenerateRandomNumberAndValidate();//When Id=0
                    }
                    else
                    {
                        //When Id = 0
                        inputDTO.IsCrm = 0; //when id=0
                        inputDTO.Status = 1; //when id=0
                        inputDTO.CreationDate = DateTime.Now; //when id=0
                        inputDTO.AprovedDate = DateTime.Now; //when id=0
                        inputDTO.IsApproved = 1;//When Id=0
                        inputDTO.ApprovedBy = Convert.ToInt32(User.FindFirstValue("Id")); //when id=0

                        inputDTO.UniqueNo = "NAAD00" + CommonHelper.GenerateNaadRandomNo();//When Id=0
                        inputDTO.UHID = await _guestsAPIController.GenerateUHIDAndValidate(dataVM?.MemberDetail?.MobileNo ?? "");//When Id=0

                        inputDTO.GroupId = await _guestsAPIController.GenerateRandomNumberAndValidate();//When Id=0
                    }
                }
                else if (String.IsNullOrEmpty(dataVM.MemberDetail.UHID))
                {
                    if (dataVM.Source == "RoomAvailability")
                    {
                        inputDTO.RoomType = await _guestsAPIController.GetRoomTypeByRoomNumber(dataVM?.RoomAllocation?.Rnumber ?? "");
                        int PAX = await _guestsAPIController.GetPaxAndUpdate(dataVM?.MemberDetail?.GroupId ?? "");
                        if (PAX <= 0)
                        {
                            if (PAX == -2)
                                return BadRequest("Room has reached the limit to add Guests.");
                        }
                        inputDTO.Pax = PAX;
                        inputDTO.NoOfRooms = 1;
                        inputDTO.PAXSno = PAX;

                        inputDTO.IsCrm = 0; //when id=0
                        inputDTO.Status = 1; //when id=0
                        inputDTO.CreationDate = DateTime.Now; //when id=0
                        inputDTO.AprovedDate = DateTime.Now; //when id=0
                        inputDTO.IsApproved = 1;//When Id=0
                        inputDTO.ApprovedBy = Convert.ToInt32(User.FindFirstValue("Id")); //when id=0
                        inputDTO.UniqueNo = "NAAD00" + CommonHelper.GenerateNaadRandomNo();//When Id=0
                        inputDTO.GroupId = dataVM?.MemberDetail?.GroupId ?? "";
                    }
                    else
                    {
                        inputDTO.IsCrm = 0; //when id=0
                        inputDTO.Status = 1; //when id=0
                        inputDTO.CreationDate = DateTime.Now; //when id=0
                        inputDTO.AprovedDate = DateTime.Now; //when id=0
                        inputDTO.IsApproved = 1;//When Id=0
                        inputDTO.ApprovedBy = Convert.ToInt32(User.FindFirstValue("Id")); //when id=0
                        inputDTO.UniqueNo = "NAAD00" + CommonHelper.GenerateNaadRandomNo();//When Id=0
                        inputDTO.UHID = await _guestsAPIController.GenerateUHIDAndValidate(dataVM?.MemberDetail?.MobileNo ?? "");//When Id=0
                        inputDTO.GroupId = dataVM.MemberDetail.GroupId;
                    }

                }
                else
                {
                    if (dataVM.Source == "RoomAvailability")
                    {
                        inputDTO.RoomType = await _guestsAPIController.GetRoomTypeByRoomNumber(dataVM?.RoomAllocation?.Rnumber ?? "");
                        int PAX = await _guestsAPIController.GetPaxAndUpdate(dataVM?.MemberDetail?.GroupId ?? "");
                        if (PAX <= 0)
                        {
                            if (PAX == -2)
                                return BadRequest("Room has reached the limit to add Guests.");
                        }
                        inputDTO.Pax = PAX;
                        inputDTO.NoOfRooms = 1;
                        inputDTO.PAXSno = PAX;

                        inputDTO.GroupId = dataVM.MemberDetail.GroupId;
                        inputDTO.UniqueNo = dataVM.MemberDetail.UniqueNo;
                    }
                    else
                    {
                        inputDTO.GroupId = dataVM.MemberDetail.GroupId;
                        inputDTO.UniqueNo = dataVM.MemberDetail.UniqueNo;
                        inputDTO.UHID = dataVM.MemberDetail.UHID;
                    }
                }
                if (String.IsNullOrEmpty(inputDTO.UniqueNo))
                {
                    inputDTO.UniqueNo = "NAAD00" + CommonHelper.GenerateNaadRandomNo();//When Id=0
                }
                if (String.IsNullOrEmpty(inputDTO.UHID))
                {
                    inputDTO.UHID = await _guestsAPIController.GenerateUHIDAndValidate(dataVM?.MemberDetail?.MobileNo ?? "");//When Id=0
                }

                //if (dataVM.MemberDetail.Id > 0)
                //{
                //    inputDTO.Id = dataVM.MemberDetail.Id;
                //}
                //else
                //{
                //    //When Id = 0
                //    inputDTO.IsCrm = 0; //when id=0
                //    inputDTO.Status = 1; //when id=0
                //    inputDTO.CreationDate = DateTime.Now; //when id=0
                //    inputDTO.AprovedDate = DateTime.Now; //when id=0
                //    inputDTO.IsApproved = 1;//When Id=0
                //    inputDTO.ApprovedBy = Convert.ToInt32(User.FindFirstValue("Id")); //when id=0
                //    inputDTO.UniqueNo = "NAAD00" + CommonHelper.GenerateNaadRandomNo();//When Id=0

                //    inputDTO.GroupId = await _guestsAPIController.GenerateRandomNumberAndValidate();//When Id=0
                //}


                #endregion


                var res = await _guestsAPIController.ManageGuests(inputDTO);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    MembersDetailsDTO? resInputDTO = (MembersDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                    if (inputDTO.PAXSno == inputDTO.Pax)
                    {
                        inputDTO.PaxCompleted = true;
                        await _guestsAPIController.UpdatePAxCompleted(inputDTO);


                        #region autoAssignRooms

                        if (resInputDTO != null && resInputDTO.opt == "Add")
                        {
                            await _guestsAPIController.AutoRoomAssign(inputDTO);
                        }





                        #endregion
                    }


                    if (dataVM.Source == "RoomAvailability")
                    {
                        RoomAllocationDTO roomAllocationDTO = new RoomAllocationDTO();
                        roomAllocationDTO.Rnumber = dataVM?.RoomAllocation?.Rnumber;
                        roomAllocationDTO.Rtype = inputDTO.RoomType;
                        roomAllocationDTO.GuestId = inputDTO.Id;
                        roomAllocationDTO.Fd = inputDTO?.DateOfArrival;
                        roomAllocationDTO.Td = inputDTO?.DateOfDepartment;
                        roomAllocationDTO.AsigndDate = DateTime.Now;
                        roomAllocationDTO.IsActive = 1;
                        roomAllocationDTO.Remarks = dataVM?.BookStatus;
                        roomAllocationDTO.CreeatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                        roomAllocationDTO.Shared = inputDTO?.PAXSno > 1 ? 1 : 2;
                        var roomAllocated = await _guestsAPIController.AllocateRoom(roomAllocationDTO);
                        if (roomAllocated != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomAllocated).StatusCode == 200)
                        {
                            return res;
                        }
                    }


                    #region Photo Attachments
                    try
                    {
                        var attachments = dataVM?.PhotoAttachment;
                        if (attachments != null)
                        {
                            foreach (var attachment in attachments)
                            {
                                if (attachment.Length > 0)
                                {
                                    var attachmentName = Path.GetFileName(attachment.FileName);
                                    var attachmentExtension = Path.GetExtension(attachmentName);
                                    string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "GuestPhoto", resInputDTO?.Id.ToString() ?? "0");
                                    string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                    if (!Directory.Exists(FilePath))
                                        Directory.CreateDirectory(FilePath);
                                    var filePathWithAttachment = Path.Combine(FilePath, attachmentName);

                                    GuestDocumentAttachmentsDTO guestDocumentAttachmentsDTO = new GuestDocumentAttachmentsDTO();
                                    guestDocumentAttachmentsDTO.AttachmentName = attachmentName;
                                    guestDocumentAttachmentsDTO.AttachmentPath = FilePathWithoutRoot;
                                    guestDocumentAttachmentsDTO.AttachmentExtension = attachmentExtension;
                                    guestDocumentAttachmentsDTO.AttachmentType = "GuestPhoto";
                                    guestDocumentAttachmentsDTO.IsActive = true;
                                    guestDocumentAttachmentsDTO.CreatedDate = DateTime.Now;
                                    guestDocumentAttachmentsDTO.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                                    guestDocumentAttachmentsDTO.AttachmentSource = "AddGuest_GuestPage";
                                    guestDocumentAttachmentsDTO.ReferenceId = resInputDTO?.Id;
                                    //Add to Database
                                    var fileUploadRes = await _guestsAPIController.SaveAttachmentDetailsToDatabase(guestDocumentAttachmentsDTO);

                                    if (fileUploadRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)fileUploadRes).StatusCode == 200)
                                    {
                                        using (FileStream fs = System.IO.File.Create(filePathWithAttachment))
                                        {
                                            attachment.CopyTo(fs);
                                        }
                                    }
                                    else
                                    {
                                        return fileUploadRes ?? BadRequest("Some error has occurred while uploading attachment");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    #endregion
                    #region IdProof Attachment

                    try
                    {
                        var attachments = dataVM?.IdProofAttachment;
                        if (attachments != null)
                        {
                            foreach (var attachment in attachments)
                            {
                                if (attachment.Length > 0)
                                {
                                    var attachmentName = Path.GetFileName(attachment.FileName);
                                    var attachmentExtension = Path.GetExtension(attachmentName);
                                    string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "GuestIdProof", resInputDTO?.Id.ToString() ?? "0");
                                    string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                    if (!Directory.Exists(FilePath))
                                        Directory.CreateDirectory(FilePath);
                                    var filePathWithAttachment = Path.Combine(FilePath, attachmentName);

                                    GuestDocumentAttachmentsDTO guestDocumentAttachmentsDTO = new GuestDocumentAttachmentsDTO();
                                    guestDocumentAttachmentsDTO.AttachmentName = attachmentName;
                                    guestDocumentAttachmentsDTO.AttachmentPath = FilePathWithoutRoot;
                                    guestDocumentAttachmentsDTO.AttachmentExtension = attachmentExtension;
                                    guestDocumentAttachmentsDTO.AttachmentType = "GuestIdProof";
                                    guestDocumentAttachmentsDTO.IsActive = true;
                                    guestDocumentAttachmentsDTO.CreatedDate = DateTime.Now;
                                    guestDocumentAttachmentsDTO.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                                    guestDocumentAttachmentsDTO.AttachmentSource = "AddGuest_GuestPage";
                                    guestDocumentAttachmentsDTO.ReferenceId = resInputDTO?.Id;
                                    //Add to Database
                                    var fileUploadRes = await _guestsAPIController.SaveAttachmentDetailsToDatabase(guestDocumentAttachmentsDTO);

                                    if (fileUploadRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)fileUploadRes).StatusCode == 200)
                                    {
                                        using (FileStream fs = System.IO.File.Create(filePathWithAttachment))
                                        {
                                            attachment.CopyTo(fs);
                                        }
                                    }
                                    else
                                    {
                                        return fileUploadRes ?? BadRequest("Some error has occurred while uploading attachment");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    #endregion
                    #region Passport Attachment

                    try
                    {
                        var attachments = dataVM?.PassportAttachment;
                        if (attachments != null)
                        {
                            foreach (var attachment in attachments)
                            {
                                if (attachment.Length > 0)
                                {
                                    var attachmentName = Path.GetFileName(attachment.FileName);
                                    var attachmentExtension = Path.GetExtension(attachmentName);
                                    string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "GuestPassport", resInputDTO?.Id.ToString() ?? "0");
                                    string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                    if (!Directory.Exists(FilePath))
                                        Directory.CreateDirectory(FilePath);
                                    var filePathWithAttachment = Path.Combine(FilePath, attachmentName);

                                    GuestDocumentAttachmentsDTO guestDocumentAttachmentsDTO = new GuestDocumentAttachmentsDTO();
                                    guestDocumentAttachmentsDTO.AttachmentName = attachmentName;
                                    guestDocumentAttachmentsDTO.AttachmentPath = FilePathWithoutRoot;
                                    guestDocumentAttachmentsDTO.AttachmentExtension = attachmentExtension;
                                    guestDocumentAttachmentsDTO.AttachmentType = "GuestPassport";
                                    guestDocumentAttachmentsDTO.IsActive = true;
                                    guestDocumentAttachmentsDTO.CreatedDate = DateTime.Now;
                                    guestDocumentAttachmentsDTO.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                                    guestDocumentAttachmentsDTO.AttachmentSource = "AddGuest_GuestPage";
                                    guestDocumentAttachmentsDTO.ReferenceId = resInputDTO?.Id;
                                    //Add to Database
                                    var fileUploadRes = await _guestsAPIController.SaveAttachmentDetailsToDatabase(guestDocumentAttachmentsDTO);

                                    if (fileUploadRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)fileUploadRes).StatusCode == 200)
                                    {
                                        using (FileStream fs = System.IO.File.Create(filePathWithAttachment))
                                        {
                                            attachment.CopyTo(fs);
                                        }
                                    }
                                    else
                                    {
                                        return fileUploadRes ?? BadRequest("Some error has occurred while uploading attachment");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    #endregion
                    #region Visa Attachment

                    try
                    {
                        var attachments = dataVM?.VisaAttachment;
                        if (attachments != null)
                        {
                            foreach (var attachment in attachments)
                            {
                                if (attachment.Length > 0)
                                {
                                    var attachmentName = Path.GetFileName(attachment.FileName);
                                    var attachmentExtension = Path.GetExtension(attachmentName);
                                    string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "GuestVisa", resInputDTO?.Id.ToString() ?? "0");
                                    string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                    if (!Directory.Exists(FilePath))
                                        Directory.CreateDirectory(FilePath);
                                    var filePathWithAttachment = Path.Combine(FilePath, attachmentName);

                                    GuestDocumentAttachmentsDTO guestDocumentAttachmentsDTO = new GuestDocumentAttachmentsDTO();
                                    guestDocumentAttachmentsDTO.AttachmentName = attachmentName;
                                    guestDocumentAttachmentsDTO.AttachmentPath = FilePathWithoutRoot;
                                    guestDocumentAttachmentsDTO.AttachmentExtension = attachmentExtension;
                                    guestDocumentAttachmentsDTO.AttachmentType = "GuestVisa";
                                    guestDocumentAttachmentsDTO.IsActive = true;
                                    guestDocumentAttachmentsDTO.CreatedDate = DateTime.Now;
                                    guestDocumentAttachmentsDTO.CreatedBy = Convert.ToInt32(User.FindFirstValue("Id"));
                                    guestDocumentAttachmentsDTO.AttachmentSource = "AddGuest_GuestPage";
                                    guestDocumentAttachmentsDTO.ReferenceId = resInputDTO?.Id;
                                    //Add to Database
                                    var fileUploadRes = await _guestsAPIController.SaveAttachmentDetailsToDatabase(guestDocumentAttachmentsDTO);

                                    if (fileUploadRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)fileUploadRes).StatusCode == 200)
                                    {
                                        using (FileStream fs = System.IO.File.Create(filePathWithAttachment))
                                        {
                                            attachment.CopyTo(fs);
                                        }
                                    }
                                    else
                                    {
                                        return fileUploadRes ?? BadRequest("Some error has occurred while uploading attachment");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    #endregion


                }



                return res;
                //if (String.IsNullOrEmpty(inputDTO.GroupId))
                //{
                //    var res = await _guestsAPIController.AddGuests(inputDTO);
                //    return res;
                //}
                //else
                //{
                //    var res = await _guestsAPIController.UpdateGuests(inputDTO);
                //    return res;
                //}
                //if (dataVM.MemberDetail.Id > 0)
                //{
                //    var res = await _guestsAPIController.UpdateGuests(inputDTO);
                //    return res;
                //}
                //else
                //{
                //    var res = await _guestsAPIController.AddGuests(inputDTO);
                //    return res;
                //}
            }
            else
            {
                return BadRequest("Data is invalid");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}
