using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Guests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;


namespace GMS.WebUI.Controllers.Guests;

public class ReservationController : Controller
{
    private readonly ILogger<ReservationController> _logger;
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
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;

    public ReservationController(ILogger<ReservationController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, GenderAPIController genderAPIController, CountryAPIController countryAPIController, ServicesAPIController servicesAPIController, MSTCategoriesAPIController mSTCategoriesAPIController, RoomTypeAPIController roomTypeAPIController, CarTypeAPIController carTypeAPIController, BrandAwarenessAPIController brandAwarenessAPIController, LeadSourceAPIController leadSourceAPIController, ChannelCodeAPIController channelCodeAPIController, GuaranteeCodeAPIController guaranteeCodeAPIController, IWebHostEnvironment hostingEnv)
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
    }

    [Route("/Reservation/GuestReservation/{queryString?}")]
    public async Task<IActionResult> GuestReservation(string? queryString)
    {
        GuestsListViewModel guestsListViewModel = new GuestsListViewModel();
        MembersDetailsDTO membersDetails = new MembersDetailsDTO();
        membersDetails.PAXSno = 1;
        if (!String.IsNullOrEmpty(queryString))
        {
            var json = Uri.UnescapeDataString(queryString ?? "");
            var obj = JsonSerializer.Deserialize<GuestReservationRouteValues>(json);

            if (obj != null)
            {
                obj.RoomType = await _guestsAPIController.GetRoomTypeByRoomNumber(obj.RoomNumber ?? "");

                membersDetails.GroupId = obj.GroupId ?? "";
                membersDetails.PAXSno = obj.PAXSno ?? 1;
                membersDetails.RoomNumber = obj.RoomNumber;
                membersDetails.RoomType = obj.RoomType;
                membersDetails.RoomNumber = obj.RoomNumber;

                guestsListViewModel.GuestReservationRouteValues = obj;
            }

        }


        guestsListViewModel.VisibleSections = new Dictionary<string, bool>
        {
            {"PersonalInformation", true},
            {"ContactInformation", true},
            {"Documents", true},
            {"StayInformation", true},
            {"EmergencySection", true},
            {"MedicalInsuranceSection", true},
            {"MarketingInformation", true},
            {"PaymentInformation", true}
        };


        guestsListViewModel.MemberDetail = membersDetails;



        return View(guestsListViewModel);
    }

    //public async Task<IActionResult> AddGuestsPartialView([FromBody] MembersDetailsDTO inputDTO)
    public async Task<IActionResult> AddGuestsPartialView([FromBody] GuestsListViewModel inputData)
    {


        GuestsListViewModel viewModel = new GuestsListViewModel();

        MembersDetailsDTO inputDTO = new MembersDetailsDTO();

        if (inputData != null && inputData.MemberDetail != null)
        {
            viewModel.GuestReservationRouteValues = inputData.GuestReservationRouteValues;
            if (inputData.MemberDetail != null)
            {
                inputDTO = inputData.MemberDetail;
            }
        }


        viewModel.VisibleSections = new Dictionary<string, bool>
        {
            {"PersonalInformation", true},
            {"ContactInformation", true},
            {"Documents", true},
            {"StayInformation", true},
            {"EmergencySection", true},
            {"MedicalInsuranceSection", true},
            {"MarketingInformation", true},
            {"PaymentInformation", true}
        };

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

        if (viewModel != null && viewModel.GuestReservationRouteValues != null)
        {
            if (viewModel.GuestReservationRouteValues.PageSource == "RoomAllocation")
            {
                if (viewModel != null && viewModel.MemberDetail != null)
                {
                    viewModel.MemberDetail.DateOfArrival = DateTime.Now.Date.AddHours(12);
                    viewModel.MemberDetail.RoomType = viewModel.GuestReservationRouteValues.RoomType;
                }
                else if (viewModel != null && viewModel.MemberDetail == null)
                {
                    viewModel.MemberDetail = new MembersDetailsDTO();
                    viewModel.MemberDetail.DateOfArrival = DateTime.Now.Date.AddHours(12);
                    viewModel.MemberDetail.RoomType = viewModel.GuestReservationRouteValues.RoomType;
                }
            }
        }


        return PartialView("_guestReservation/_guestReservationForm", viewModel);
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

        var PersonalInformation = RenderPartialViewToString("~/Views/Reservation/", "_guestReservationForm/_personalInformation", viewModel, ControllerContext);
        var ContactInformation = RenderPartialViewToString("~/Views/Reservation/", "_guestReservationForm/_contactInformation", viewModel, ControllerContext);
        var Documents = RenderPartialViewToString("~/Views/Reservation/", "_guestReservationForm/_documents", viewModel, ControllerContext);
        var HiddenValues = RenderPartialViewToString("~/Views/Reservation/", "_guestReservationForm/_hiddenValues", viewModel, ControllerContext);

        return Json(new
        {
            PersonalInformation = PersonalInformation,
            ContactInformation = ContactInformation,
            Documents = Documents,
            HiddenValues = HiddenValues
        });

        //return PartialView("_guestsList/_guestsFormDetails", viewModel);
    }


    private string RenderPartialViewToString(string viewPath, string viewName, object model, ControllerContext controllerContext)
    {
        var viewEngine = controllerContext.HttpContext.RequestServices.GetService<ICompositeViewEngine>();

        // Try multiple resolution strategies
        var viewResult = viewEngine.FindView(controllerContext, viewName, isMainPage: false)
                   ?? viewEngine.GetView(viewPath, $"{viewName}.cshtml", isMainPage: false);

        if (!viewResult.Success)
        {
            throw new Exception($"View not found. Searched:\n{string.Join("\n", viewResult.SearchedLocations)}");
        }

        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), controllerContext.ModelState)
        {
            Model = model
        };

        using (var writer = new StringWriter())
        {
            var viewContext = new ViewContext(
                controllerContext,
                viewResult.View,
                viewData,
                new TempDataDictionary(controllerContext.HttpContext,
                    controllerContext.HttpContext.RequestServices.GetService<ITempDataProvider>()),
                writer,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
            return writer.ToString();
        }
    }

}
