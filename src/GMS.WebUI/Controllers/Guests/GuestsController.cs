using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Endpoints.Rooms;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.Accounting;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.Rooms;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;



namespace GMS.WebUI.Controllers.Guests;

public class GuestsController : Controller
{
    private readonly ILogger<GuestsController> _logger;
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

    public GuestsController(ILogger<GuestsController> logger, GuestsAPIController guestsAPIController, AccountsAPIController accountsAPIController, GenderAPIController genderAPIController, CountryAPIController countryAPIController, ServicesAPIController servicesAPIController, MSTCategoriesAPIController mSTCategoriesAPIController, RoomTypeAPIController roomTypeAPIController, CarTypeAPIController carTypeAPIController, BrandAwarenessAPIController brandAwarenessAPIController, LeadSourceAPIController leadSourceAPIController, ChannelCodeAPIController channelCodeAPIController, GuaranteeCodeAPIController guaranteeCodeAPIController, IWebHostEnvironment hostingEnv)
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
    public IActionResult Index()
    {
        return View();
    }
    [Authorize]
    public async Task<IActionResult> GuestsList()
    {
        //var res = await _guestsAPIController.GetGuestsList();
        //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //{
        //    List<GMSFinalGuestDTO>? dto = (List<GMSFinalGuestDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //    return View(dto);
        //}
        return View();
    }
    public async Task<IActionResult> GuestsGridViewPartialView([FromBody] GuestsGridViewParameters inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();

        viewModel = await GuestsDataForGridViewPartialView(inputDTO);

        return PartialView("_guestsList/_guestsGridView", viewModel);

        var res = await _guestsAPIController.GuestsListStatusWiseAndPageWise(inputDTO, "Data");
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.MemberDetailsWithChildren = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                //if (viewModel.MemberDetailsWithChildren != null)
                //{
                //    foreach (var item in viewModel.MemberDetailsWithChildren)
                //    {
                //        if (item != null)
                //        {
                //            //item.encProjectID = CommonHelper.EncryptURLHTML(item.ProjectID.ToString());
                //        }
                //    }
                //}
            }
        }

        var resCount = await _guestsAPIController.GuestsListStatusWiseAndPageWise(inputDTO, "Count");
        if (resCount != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).StatusCode == 200)
        {
            GuestsGridViewParameters pageDetails = new GuestsGridViewParameters();
            var totalRecords = (List<int>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).Value;
            pageDetails.TotalRecords = totalRecords == null ? 0 : totalRecords.Count == 0 ? 0 : totalRecords[0];

            pageDetails.PageSize = (inputDTO != null && inputDTO.PageSize != null) ? inputDTO.PageSize : 10;
            pageDetails.PageNumber = (inputDTO != null && inputDTO.PageNumber != null) ? inputDTO.PageNumber : 1;
            pageDetails.TotalPages = (int)Math.Ceiling((double?)pageDetails.TotalRecords / pageDetails.PageSize ?? default(int));
            pageDetails.SearchKeyword = inputDTO == null ? "" : String.IsNullOrEmpty(inputDTO.SearchKeyword) ? "" : inputDTO.SearchKeyword;
            viewModel.GuestsGridViewParameters = pageDetails;
        }

        return PartialView("_guestsList/_guestsGridView", viewModel);
    }

    public async Task<IActionResult> GuestsTableViewPartialView([FromBody] GuestsGridViewParameters inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();

        viewModel = await GuestsDataForGridViewPartialView(inputDTO);

        return PartialView("../Home/_index/_guestsList", viewModel);
    }

    public async Task<GuestsListViewModel> GuestsDataForGridViewPartialView([FromBody] GuestsGridViewParameters inputDTO)
    {
        GuestsListViewModel viewModel = new GuestsListViewModel();
        var res = await _guestsAPIController.GuestsListStatusWiseAndPageWise(inputDTO, "Data");
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.MemberDetailsWithChildren = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                //if (viewModel.MemberDetailsWithChildren != null)
                //{
                //    foreach (var item in viewModel.MemberDetailsWithChildren)
                //    {
                //        if (item != null)
                //        {
                //            //item.encProjectID = CommonHelper.EncryptURLHTML(item.ProjectID.ToString());
                //        }
                //    }
                //}
            }
        }

        var resCount = await _guestsAPIController.GuestsListStatusWiseAndPageWise(inputDTO, "Count");
        if (resCount != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).StatusCode == 200)
        {
            GuestsGridViewParameters pageDetails = new GuestsGridViewParameters();
            var totalRecords = (List<int>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).Value;
            pageDetails.TotalRecords = totalRecords == null ? 0 : totalRecords.Count == 0 ? 0 : totalRecords[0];

            pageDetails.PageSize = (inputDTO != null && inputDTO.PageSize != null) ? inputDTO.PageSize : 10;
            pageDetails.PageNumber = (inputDTO != null && inputDTO.PageNumber != null) ? inputDTO.PageNumber : 1;
            pageDetails.TotalPages = (int)Math.Ceiling((double?)pageDetails.TotalRecords / pageDetails.PageSize ?? default(int));
            pageDetails.SearchKeyword = inputDTO == null ? "" : String.IsNullOrEmpty(inputDTO.SearchKeyword) ? "" : inputDTO.SearchKeyword;
            viewModel.GuestsGridViewParameters = pageDetails;
        }

        return viewModel;
    }
    public async Task<IActionResult> GuestCheckOutPartialView([FromBody] MembersDetailsDTO inputDTO)
    {
        GuestsCheckListViewModel viewModel = new GuestsCheckListViewModel();

        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(inputDTO.Id);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            viewModel.MemberDetails = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }

        var resCheckListOut = await _guestsAPIController.GetGuestCheckList("CheckOut");
        if (resCheckListOut != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCheckListOut).StatusCode == 200)
        {
            viewModel.CheckListOut = (List<TblCheckListsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCheckListOut).Value;
        }

        return PartialView("_guestsList/_guestsCheckOut", viewModel);
    }
    public async Task<IActionResult> CheckSettlementBeforeCheckoutEligibility([FromBody] MembersDetailsDTO inputDTO1)
    {
        var accountSettled = await _guestsAPIController.AccountSettled(inputDTO1.Id);
        if (accountSettled)
        {
            return Ok("");
        }
        else
        {
            // Check if there's a partial settlement with unapproved debit note
            var settlementRes = await _guestsAPIController.GetSettlementData(inputDTO1.Id);
            SettlementDTO? settlement = null;
            if (settlementRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)settlementRes).StatusCode == 200)
            {
                settlement = (SettlementDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)settlementRes).Value;
            }
            
            if (settlement != null && settlement.Status == 1 && settlement.DebitAmount > 0 && settlement.DebitNoteIsApproved != true)
            {
                string debitNoteNumber = settlement.DebitNoteNumber ?? "";
                return BadRequest(new { heading = "Debit Note Pending Approval", message = $"Checkout cannot be processed. Debit Note {debitNoteNumber} is pending approval from accounts." });
            }
            else
            {
                return BadRequest(new { heading = "Pending Account Settlement", message = "Checkout cannot be processed until all outstanding charges are cleared" });
            }
        }

    }
    public async Task<IActionResult> GuestCheckInPartialView([FromBody] MembersDetailsDTO inputDTO)
    {
        GuestsCheckListViewModel viewModel = new GuestsCheckListViewModel();

        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(inputDTO.Id);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            viewModel.MemberDetails = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }

        var resCheckListIn = await _guestsAPIController.GetGuestCheckList("CheckIn");
        if (resCheckListIn != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCheckListIn).StatusCode == 200)
        {
            viewModel.CheckListOut = (List<TblCheckListsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCheckListIn).Value;
        }

        return PartialView("_guestsList/_guestsCheckIn", viewModel);
    }


    public async Task<IActionResult> AddGuestsPartialView([FromBody] MembersDetailsDTO inputDTO)
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
        return PartialView("_guestsList/_addGuests", viewModel);
    }

    public async Task<IActionResult> GetCategoriesByServiceId([FromBody] ServicesDTO inputDTO)
    {
        var servicesRes = await _mSTCategoriesAPIController.CategoriesList(inputDTO.Id);
        return servicesRes;
    }
    public async Task<IActionResult> FetchDepartmentDate([FromBody] MembersDetailsDTO inputDTO)
    {
        var res = await _guestsAPIController.FetchDepartmentDate(inputDTO);
        return res;
    }
    [HttpPost]
    public async Task<IActionResult> SaveMemberDetails(GuestsListViewModel dataVM)
    {
        //return Ok("");
        try
        {
            if (dataVM != null && dataVM.MemberDetail != null)
            {
                #region NormalizeData Guest Data

                dataVM.MemberDetail.MobileNo = CommonHelper.NormalizePhoneNumber(dataVM?.MemberDetail.MobileNo ?? "");

                string fullname = string.Join(" ", new[] {
                                        dataVM?.MemberDetail?.Fname,
                                        dataVM?.MemberDetail?.Mname,
                                        dataVM?.MemberDetail?.Lname
                                    }.Where(x => !string.IsNullOrWhiteSpace(x)));

                #endregion


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
                inputDTO.MobileNo = CommonHelper.NormalizePhoneNumber(dataVM.MemberDetail.MobileNo ?? "");
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

                        inputDTO.UHID = await _guestsAPIController.GenerateUHIDAndValidate(dataVM?.MemberDetail?.MobileNo ?? "", fullname);//When Id=0

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
                        inputDTO.UHID = await _guestsAPIController.GenerateUHIDAndValidate(dataVM?.MemberDetail?.MobileNo ?? "", fullname);//When Id=0
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
                    inputDTO.UHID = await _guestsAPIController.GenerateUHIDAndValidate(dataVM?.MemberDetail?.MobileNo ?? "", fullname);//When Id=0
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
                            bool roomAssigned = await _guestsAPIController.AutoRoomAssign(inputDTO);
                            if (roomAssigned)
                            {
                                inputDTO.LoggedInUser = Convert.ToInt32(User.FindFirstValue("Id"));
                                await _guestsAPIController.PostChargesToAuditBySP(inputDTO.Id);
                            }
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
                            try
                            {
                                inputDTO.LoggedInUser = Convert.ToInt32(User.FindFirstValue("Id"));
                                await _guestsAPIController.PostChargesToAuditBySP(inputDTO.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error calling InsertAuditRevenueForGuest stored procedure for GuestId: {inputDTO.Id} during manual room allocation in {nameof(SaveMemberDetails)}");
                                // Continue execution even if audit revenue fails - guest registration should not fail
                            }
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

    public async Task<IActionResult> GetRoomsAvailableForGuestsSharedNew([FromBody] RoomAllocationDTO inputDTO)
    {
        List<AvailableRoomsForGuestAllocation>? dto = new List<AvailableRoomsForGuestAllocation>();
        if (inputDTO != null && inputDTO.Shared == 1)
        {
            var roomNumbers = await _guestsAPIController.GetRoomsAvailableForGuestsShared(inputDTO.GuestId ?? 0);
            if (roomNumbers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).StatusCode == 200)
            {
                dto = (List<AvailableRoomsForGuestAllocation>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).Value;
            }
        }
        else
        {
            var roomNumbers = await _guestsAPIController.GetRoomsAvailableForGuestsNonShared(inputDTO.GuestId ?? 0);
            if (roomNumbers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).StatusCode == 200)
            {
                dto = (List<AvailableRoomsForGuestAllocation>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).Value;
            }
        }
        return Ok(dto);
    }

    public async Task<IActionResult> GetRoomsAvailableForGuestsByRoomType([FromBody] RoomAllocationDTO inputDTO)
    {
        List<AvailableRoomsForGuestAllocation>? dto = new List<AvailableRoomsForGuestAllocation>();
        if (inputDTO != null && inputDTO.GuestId > 0 && inputDTO.Rtype > 0)
        {
            if (inputDTO.Shared == 1)
            {
                var roomNumbers = await _guestsAPIController.GetRoomsAvailableForGuestsSharedByRoomType(inputDTO.GuestId ?? 0, inputDTO.Rtype ?? 0);
                if (roomNumbers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).StatusCode == 200)
                {
                    dto = (List<AvailableRoomsForGuestAllocation>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).Value;
                }
            }
            else
            {
                var roomNumbers = await _guestsAPIController.GetRoomsAvailableForGuestsNonSharedByRoomType(inputDTO.GuestId ?? 0, inputDTO.Rtype ?? 0);
                if (roomNumbers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).StatusCode == 200)
                {
                    dto = (List<AvailableRoomsForGuestAllocation>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).Value;
                }
            }
        }
        return Ok(dto);
    }

    public async Task<IActionResult> AddRoomPartialView([FromBody] MembersDetailsDTO inputDTO)
    {
        MembersDetailsDTO? membersDetailsDTO = new MembersDetailsDTO();
        AddRoomPopupViewModel viewModel = new AddRoomPopupViewModel();
        if (inputDTO.Id > 0)
        {
            var roomAllocation = await _guestsAPIController.GetRoomAllocationByGuestId(inputDTO.Id);
            if (roomAllocation != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomAllocation).StatusCode == 200)
            {
                viewModel.RoomAllocationDetails = (RoomAllocationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomAllocation).Value;

                if (viewModel.RoomAllocationDetails != null && viewModel.RoomAllocationDetails.Shared == 1)
                {
                    var roomNumbers = await _guestsAPIController.GetRoomsAvailableForGuestsShared(inputDTO.Id);
                    if (roomNumbers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).StatusCode == 200)
                    {
                        viewModel.AvailableRoomsForGuestAllocationList = (List<AvailableRoomsForGuestAllocation>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).Value;
                    }
                }
                else
                {
                    var roomNumbers = await _guestsAPIController.GetRoomsAvailableForGuestsNonShared(inputDTO.Id);
                    if (roomNumbers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).StatusCode == 200)
                    {
                        viewModel.AvailableRoomsForGuestAllocationList = (List<AvailableRoomsForGuestAllocation>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).Value;
                    }
                }
            }
            else
            {
                var roomNumbers = await _guestsAPIController.GetRoomsAvailableForGuestsNonShared(inputDTO.Id);
                if (roomNumbers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).StatusCode == 200)
                {
                    viewModel.AvailableRoomsForGuestAllocationList = (List<AvailableRoomsForGuestAllocation>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomNumbers).Value;
                }
            }
            var roomPArtnerDetails = await _guestsAPIController.GetGuestListSharingRoomByGuestId(inputDTO.Id);
            if (roomPArtnerDetails != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomPArtnerDetails).StatusCode == 200)
            {
                viewModel.RoomPartnerName = (List<MembersDetailsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomPArtnerDetails).Value;
                if (viewModel.RoomPartnerName != null && viewModel.RoomPartnerName.Count > 0)
                {
                    viewModel.isShared = true;
                }
            }

            var guestRes = await _guestsAPIController.GetGuestByIdWithChild(inputDTO.Id);
            if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
            {
                viewModel.MembersDetailWithChild = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
            }

            // Load RoomTypes
            var roomTypes = await _roomTypeAPIController.List();
            if (roomTypes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).StatusCode == 200)
            {
                viewModel.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).Value;
            }

            //var sharedStatusRes = await _guestsAPIController.GetAvailableRoomsSharedStatusForGuest(inputDTO.Id);
            //if (sharedStatusRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)sharedStatusRes).StatusCode == 200)
            //{
            //    List<AvailableRoomsSharedStatus>? availableRoomsSharedStatuses = (List<AvailableRoomsSharedStatus>?)((Microsoft.AspNetCore.Mvc.ObjectResult)sharedStatusRes).Value;
            //    if (availableRoomsSharedStatuses != null && availableRoomsSharedStatuses.Count > 0)
            //    {
            //        viewModel.AvailableRoomsSharedStatus = availableRoomsSharedStatuses.FirstOrDefault();
            //    }
            //}
        }
        return PartialView("_guestsList/_addRoom", viewModel);
    }

    //public async Task<IActionResult> AllocateRoomToGuestNew([FromBody] RoomAllocationDTO inputDTO)
    //{
    //    return Ok();
    //}

    public async Task<IActionResult> AddRoomPartialView1([FromBody] MembersDetailsDTO inputDTO)
    {
        MembersDetailsDTO? membersDetailsDTO = new MembersDetailsDTO();
        AddRoomPopupViewModel viewModel = new AddRoomPopupViewModel();
        if (inputDTO.Id > 0)
        {
            var guestRes = await _guestsAPIController.GetGuestById(inputDTO.Id);
            if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
            {
                viewModel.MembersDetails = (MembersDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
            }
            var sharedStatusRes = await _guestsAPIController.GetAvailableRoomsSharedStatusForGuest(inputDTO.Id);
            if (sharedStatusRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)sharedStatusRes).StatusCode == 200)
            {
                List<AvailableRoomsSharedStatus>? availableRoomsSharedStatuses = (List<AvailableRoomsSharedStatus>?)((Microsoft.AspNetCore.Mvc.ObjectResult)sharedStatusRes).Value;
                if (availableRoomsSharedStatuses != null && availableRoomsSharedStatuses.Count > 0)
                {
                    viewModel.AvailableRoomsSharedStatus = availableRoomsSharedStatuses.FirstOrDefault();
                }
            }
        }
        return PartialView("_guestsList/_addRoom", viewModel);
    }
    public async Task<IActionResult> GetAvailableRoomsForGuestAllocation([FromBody] RoomAllocationDTO inputDTO)
    {
        AddRoomPopupViewModel viewModel = new AddRoomPopupViewModel();
        if (inputDTO.Shared > 0)
        {
            var availableRoomsRes = await _guestsAPIController.GetAvailableRoomsForGuestAllocation_New(inputDTO.GuestId ?? default(int), inputDTO.Shared ?? default(int));
            //if (availableRoomsRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)availableRoomsRes).StatusCode == 200)
            //{
            //    List<AvailableRoomsSharedStatus>? availableRoomsSharedStatuses = (List<AvailableRoomsSharedStatus>?)((Microsoft.AspNetCore.Mvc.ObjectResult)sharedStatusRes).Value;
            //    if (availableRoomsSharedStatuses != null && availableRoomsSharedStatuses.Count > 0)
            //    {
            //        viewModel.AvailableRoomsSharedStatus = availableRoomsSharedStatuses.FirstOrDefault();
            //    }
            //}
            return availableRoomsRes;
        }
        return BadRequest("Unable to fetch rooms");
    }

    [Route("/Guests/GuestRegistrationSuccessfull/{GroupId}")]
    public async Task<IActionResult> GuestRegistrationSuccessfull(string? GroupId)
    {
        if (GroupId == null)
        {
            return View();
        }
        else
        {
            List<MembersDetailsDTO>? membersDetailsDTOs = new List<MembersDetailsDTO>();
            var res = await _guestsAPIController.GetGuestByGroupId(GroupId);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (List<MembersDetailsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return View(membersDetailsDTOs);
        }
    }
    [Route("/Guests/PrintMemberDetails/{GuestId}")]
    public async Task<IActionResult> PrintMemberDetails(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);

            }
            return View(membersDetailsDTOs);
        }
    }
    [Route("/Guests/ReviewMemberDetails/{GuestId}")]
    public async Task<IActionResult> ReviewMemberDetails(int? GuestId, string section = "Review")
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);

            }
            ViewBag.Section = section;
            ViewBag.GuestId = GuestId;
            return View(membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/Documents")]
    public async Task<IActionResult> Documents(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "Documents";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/Schedules")]
    public async Task<IActionResult> Schedules(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "Schedules";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/CaseSheet")]
    public async Task<IActionResult> CaseSheet(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "CaseSheet";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/DietChart")]
    public async Task<IActionResult> DietChart(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "DietChart";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/DoctorNotes")]
    public async Task<IActionResult> DoctorNotes(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "DoctorNotes";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/DischargeSummary")]
    public async Task<IActionResult> DischargeSummary(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "DischargeSummary";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/FollowUpSheet")]
    public async Task<IActionResult> FollowUpSheet(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "FollowUpSheet";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/GuestEducation")]
    public async Task<IActionResult> GuestEducation(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "GuestEducation";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/WellnessTherapist")]
    public async Task<IActionResult> WellnessTherapist(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "WellnessTherapist";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/WorkSheet")]
    public async Task<IActionResult> WorkSheet(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "WorkSheet";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }

    [Route("/Guests/ReviewMemberDetails/{GuestId}/WellnessSchedule")]
    public async Task<IActionResult> WellnessSchedule(int? GuestId)
    {
        if (GuestId == null)
        {
            return View();
        }
        else
        {
            MemberDetailsWithChild? membersDetailsDTOs = new MemberDetailsWithChild();
            var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                membersDetailsDTOs = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (membersDetailsDTOs != null && membersDetailsDTOs.Dob != null)
                    membersDetailsDTOs.Age = CommonHelper.CalculateAge(membersDetailsDTOs.Dob);
            }
            ViewBag.Section = "WellnessSchedule";
            ViewBag.GuestId = GuestId;
            return View("ReviewMemberDetails", membersDetailsDTOs);
        }
    }
    public async Task<IActionResult> AllocateRoom([FromBody] RoomAllocationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _guestsAPIController.AllocateRoom(inputDTO);
            return res;
        }
        return BadRequest("Unable to assign room at the moment");
    }
    public async Task<IActionResult> MarkGuestNoShow([FromBody] RoomAllocationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _guestsAPIController.MarkGuestNoShow(inputDTO);
            return res;
        }
        return BadRequest("Unable to assign room at the moment");
    }
    public async Task<IActionResult> GetCancellationVerification(int guestId)
    {
        if (guestId <= 0)
        {
            return BadRequest("Invalid guest details");
        }

        var res = await _guestsAPIController.GetCancellationVerification(guestId);
        return res;
    }
    public async Task<IActionResult> VerifyCancellationAmount([FromBody] GuestCancellationVerificationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var userId = User.FindFirstValue("Id");
            if (!string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out var parsedUserId))
            {
                inputDTO.VerifiedBy = parsedUserId;
            }

            var res = await _guestsAPIController.VerifyCancellationAmount(inputDTO);
            return res;
        }

        return BadRequest("Unable to verify the cancellation amount at the moment");
    }
    public async Task<IActionResult> CancelGuestReservation([FromBody] RoomAllocationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var userId = User.FindFirstValue("Id");
            if (!string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out var parsedUserId))
            {
                inputDTO.CancelledBy = parsedUserId;
            }
            var res = await _guestsAPIController.CancelGuestReservation(inputDTO);
            return res;
        }
        return BadRequest("Unable to cancel reservation at the moment");
    }
    public async Task<IActionResult> AllocateRoomToAllGroup([FromBody] RoomAllocationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _guestsAPIController.AllocateRoomToAllGroup(inputDTO);
            return res;
        }
        return BadRequest("Unable to assign room at the moment");
    }
    public async Task<IActionResult> ChangeRoomForCurrentGuestWithSharingStatus([FromBody] RoomAllocationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _guestsAPIController.ChangeRoomForCurrentGuestWithSharingStatus(inputDTO);
            return res;
        }
        return BadRequest("Unable to assign room at the moment");
    }
    public async Task<IActionResult> ChangeRoomForCurrentGuestWithNonSharingStatus([FromBody] RoomAllocationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _guestsAPIController.ChangeRoomForCurrentGuestWithNonSharingStatus(inputDTO);
            return res;
        }
        return BadRequest("Unable to assign room at the moment");
    }

    public async Task<IActionResult> CheckInGuest([FromBody] MedicalSoultion_GuestCheckList inputDTO)
    {
        var res = await _guestsAPIController.GuestCheckInNew(inputDTO);
        return res;
    }
    public async Task<IActionResult> CheckOutGuest([FromBody] MedicalSoultion_GuestCheckList inputDTO)
    {
        var res = await _guestsAPIController.GuestCheckOut(inputDTO);
        return res;
    }

    //public async Task<IActionResult> CheckInGuest([FromBody] MedicalSoultion_GuestCheckList inputDTO)
    //{
    //    var res = await _guestsAPIController.GuestCheckOut(inputDTO);
    //    return res;
    //}

    [Route("/Guests/GuestSchedule/{GuestId?}")]
    public async Task<IActionResult> GuestSchedule(int? GuestId)
    {
        MemberDetailsWithChild? dto = new MemberDetailsWithChild();
        var res = await _guestsAPIController.GetGuestByIdWithChild(GuestId ?? default(int));
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetGuestsEventForCalender(int? GuestId)
    {
        //GuestSchedule? dto = new GuestSchedule();
        var res = await _guestsAPIController.GetGuestsEventForCalender(GuestId ?? default(int));
        //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //{
        //    dto = (GuestSchedule?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //}
        //return View(dto);
        return res;
    }

    public async Task<IActionResult> GetGuestsEventForCalenderById([FromBody] GuestScheduleDTO inputDTO)
    {
        GuestScheduleViewModel? dto = new GuestScheduleViewModel();
        var res = await _guestsAPIController.GetGuestsEventForCalenderById(inputDTO.Id);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.GuestSchedule = (GuestScheduleDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        var tasks = await _guestsAPIController.GetTasks();
        if (tasks != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)tasks).StatusCode == 200)
        {
            dto.Tasks = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)tasks).Value;
        }
        return Ok(dto);
    }
    public async Task<IActionResult> CreateGuestScheduleByCalendar([FromBody] GuestScheduleDTO inputDTO)
    {
        if (inputDTO != null)
        {
            //return Ok();

            var res = await _guestsAPIController.CreateGuestScheduleByCalendar(inputDTO);
            return res;
        }
        else { return BadRequest("Unbale to add right now"); }

    }
    public async Task<IActionResult> GetTaskName()
    {
        var res = await _guestsAPIController.GetTasks();
        return res;
    }
    public async Task<IActionResult> GetEmployeeByTaskId([FromBody] TaskMasterDTO inputDTO)
    {
        var res = await _guestsAPIController.GetEmployeeByTaskId(inputDTO.Id);
        return res;
    }
    public async Task<IActionResult> GetResourcesByTaskId([FromBody] TaskMasterDTO inputDTO)
    {
        var res = await _guestsAPIController.GetResourcesByTaskId(inputDTO.Id);
        return res;
    }
    public async Task<IActionResult> GetTaskByTaskId([FromBody] TaskMasterDTO inputDTO)
    {
        var res = await _guestsAPIController.GetTaskByTaskId(inputDTO.Id);
        return res;
    }
    public async Task<IActionResult> SearchGuestDetailsByPhoneNumber([FromBody] MembersDetailsDTO inputDTO)
    {
        List<MemberDetailsWithChild>? memberDetails = new List<MemberDetailsWithChild>();
        var res = await _guestsAPIController.GetGuestDetailsByPhoneNumber(inputDTO);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            memberDetails = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_guestsList/_guestsSearch", memberDetails);
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
        return PartialView("_guestsList/_guestsFormDetails", viewModel);
    }

    public async Task<IActionResult> ValidateRoomsAvailability([FromBody] MembersDetailsDTO inputDTO)
    {
        if (inputDTO != null)
        {
            if (inputDTO.DateOfArrival != null && inputDTO.DateOfDepartment != null && inputDTO.Pax != null && inputDTO.NoOfRooms != null && inputDTO.RoomType != null && inputDTO.Pax > 0 && inputDTO.NoOfRooms > 0 && inputDTO.RoomType > 0)
            {
                if (inputDTO.DateOfArrival != null && inputDTO.DateOfDepartment != null && inputDTO.DateOfArrival > inputDTO.DateOfDepartment)
                {
                    return BadRequest("Date Of Arrival can not be greator then Date Of Department");
                }

                float pax = float.TryParse(inputDTO.Pax.ToString(), out pax) == true ? pax : 0;
                float noofrooms = float.TryParse(inputDTO.NoOfRooms.ToString(), out noofrooms) == true ? noofrooms : 0;
                if (pax == 0 || noofrooms == 0)
                {
                    return BadRequest("PAX or No Of Rooms are invalid");
                }
                float paxperroom = pax / noofrooms;
                if (paxperroom > 3)
                {
                    return BadRequest("One room can afford only 3 person");
                }
                DateTime? DateOfDepartment = inputDTO.DateOfDepartment;
                DateTime? DateOfArrival = inputDTO.DateOfArrival;
                int? PAX = inputDTO.Pax;
                int? NoOfRooms = inputDTO.NoOfRooms;
                int? RoomType = inputDTO.RoomType;

                var res = await _guestsAPIController.IsRoomsAvailable(inputDTO);
                return res;
            }
            else
            {
                return BadRequest("data is invalid");
            }
        }
        else
        {
            return BadRequest("data is invalid");
        }
        return BadRequest("Some error occurred while validating rooms");
    }
    public async Task<IActionResult> FetchState([FromBody] TblCountriesDTO inputDTO)
    {
        var res = await _guestsAPIController.FetchState(inputDTO);
        return res;
    }
    public async Task<IActionResult> FetchCity([FromBody] TblStateDTO inputDTO)
    {
        var res = await _guestsAPIController.FetchCity(inputDTO);
        return res;
    }
    public async Task<IActionResult> DeleteUploadedFile([FromBody] GuestDocumentAttachmentsDTO inputDTO)
    {
        try
        {

            if (inputDTO != null)
            {
                var res = await _guestsAPIController.DeleteGuestAttachmentById(inputDTO.Id);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet]
    [Route("Guests/DownloadFile/{eFileId}")]
    public async Task<IActionResult> DownloadFile(string eFileId)
    {
        GuestDocumentAttachmentsDTO? inputDTO = new GuestDocumentAttachmentsDTO();
        if (eFileId != null)
        {
            int FileID = Convert.ToInt32(CommonHelper.DecryptURLHTML(eFileId));
            var res = await _guestsAPIController.GuestAttachmentByAttachmentId(FileID);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    inputDTO = (GuestDocumentAttachmentsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
        }

        if (inputDTO != null)
        {
            string _filePath = Path.Combine(_hostingEnv.WebRootPath, inputDTO?.AttachmentPath ?? "", inputDTO?.AttachmentName ?? "");
            if (!System.IO.File.Exists(_filePath))
            {
                return NotFound();
            }
            // Create a FileStream to read the file
            var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            // Determine the content type based on the file extension
            var contentType = CommonHelper.getContentTypeByExtesnion(inputDTO?.AttachmentExtension == null ? "" : inputDTO.AttachmentExtension);
            var fileName = Path.GetFileName(_filePath);

            // Return FileStreamResult with the file stream and content type
            return File(stream, contentType, fileName);
        }
        else
        {
            return NotFound();
        }
    }
    #region Billing
    public async Task<IActionResult> BillingPartialView([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();
        dto.AccountSettled = false;
        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(inputDTO?.Id ?? 0);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.MembersWithAttributes = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }
        var packagesRes = await _guestsAPIController.GetPackagesForBilling(inputDTO?.Id ?? 0);
        if (packagesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)packagesRes).StatusCode == 200)
        {
            dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)packagesRes).Value;
        }
        var tasksRes = await _guestsAPIController.GetTasksForBilling(inputDTO?.Id ?? 0);
        if (tasksRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)tasksRes).StatusCode == 200)
        {
            dto.Tasks = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)tasksRes).Value;
        }
        if (inputDTO != null)
        {
            var billingRes = await _guestsAPIController.GetBillingDataWithAttr(inputDTO.Id);
            if (billingRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)billingRes).StatusCode == 200)
            {
                dto.BillingDataList = (List<BillingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)billingRes).Value;
            }
            var allGuestsInRoomRes = await _guestsAPIController.GetAllGuestsInRoom(inputDTO.Id);
            if (allGuestsInRoomRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).StatusCode == 200)
            {
                dto.MemberDetailsWithChildren = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).Value;
            }
            var settlementRes = await _guestsAPIController.GetSettlementData(inputDTO.Id);
            if (settlementRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)settlementRes).StatusCode == 200)
            {
                dto.SettlementDTO = (SettlementDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)settlementRes).Value;
            }
            dto.AccountSettled = await _guestsAPIController.IsAccountSettled(inputDTO?.Id ?? 0);
            
            // Call stored procedure to update audit revenue when billing is opened
            try
            {
                int guestIdForAudit = inputDTO?.Id ?? 0;
                if (guestIdForAudit > 0)
                {
                    await _guestsAPIController.PostChargesToAuditBySP(guestIdForAudit);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling InsertAuditRevenueForGuest stored procedure for GuestId: {inputDTO?.Id} when opening billing in {nameof(BillingPartialView)}");
                // Continue execution even if audit revenue fails - billing view should still load
            }
        }

        return PartialView("_guestsList/_billing", dto);
    }
    public async Task<IActionResult> PaymentPartialView([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();
        dto.opt = inputDTO?.opt ?? "";
        var paymentMethodRes = await _guestsAPIController.GetPaymentMethodForBilling();
        if (paymentMethodRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)paymentMethodRes).StatusCode == 200)
        {
            dto.PaymentMethods = (List<GuaranteeCodeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)paymentMethodRes).Value;
        }
        if (inputDTO != null)
        {
            var paymentRes = await _guestsAPIController.GetPaymentDataWithAttr(inputDTO.Id);
            if (paymentRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)paymentRes).StatusCode == 200)
            {
                dto.PaymentsWithAttr = (List<PaymentWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)paymentRes).Value;
            }
        }

        return PartialView("_guestsList/_payment", dto);
    }
    public async Task<IActionResult> GetServicesForBilling([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();

        var guestRes = await _guestsAPIController.GetServicesForBilling(inputDTO?.Id ?? 0);
        //if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        //{
        //    dto.Services = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        //}
        var tasksRes = await _guestsAPIController.GetTasksForBilling(inputDTO?.Id ?? 0);
        if (tasksRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)tasksRes).StatusCode == 200)
        {
            dto.Tasks = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)tasksRes).Value;
        }
        var allGuestsInRoomRes = await _guestsAPIController.GetAllGuestsInRoom(inputDTO.Id);
        if (allGuestsInRoomRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).StatusCode == 200)
        {
            dto.MemberDetailsWithChildren = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).Value;
        }
        return Ok(dto);
    }
    public async Task<IActionResult> GetPackagesForBilling([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();
        //var guestRes = await _guestsAPIController.GetPackagesForBilling(inputDTO?.Id ?? 0);
        //if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        //{
        //    dto.Services = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        //}
        var packagesRes = await _guestsAPIController.GetPackagesForBilling(inputDTO?.Id ?? 0);
        if (packagesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)packagesRes).StatusCode == 200)
        {
            dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)packagesRes).Value;
        }
        var allGuestsInRoomRes = await _guestsAPIController.GetAllGuestsInRoom(inputDTO.Id);
        if (allGuestsInRoomRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).StatusCode == 200)
        {
            dto.MemberDetailsWithChildren = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).Value;
        }
        return Ok(dto);
        //return guestRes;
    }
    public async Task<IActionResult> GetPaymentModesForBilling([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();
        var res = await _guestsAPIController.GetPaymentMethodForBilling();
        return res;
    }
    public async Task<IActionResult> SaveBillingData([FromBody] BillingViewModel? inputDTO)
    {
        if (inputDTO != null)
        {
            int loginId = Convert.ToInt32(User.FindFirstValue("Id"));
            var res = await _guestsAPIController.SaveBillingData(inputDTO, loginId);
            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }

    public async Task<IActionResult> ConfirmAndSaveBillingData([FromBody] BillingDTO? inputDTO)
    {
        if (inputDTO != null)
        {

            int loginId = Convert.ToInt32(User.FindFirstValue("Id"));
            var res = await _guestsAPIController.ConfirmAndSaveBillingData(inputDTO, loginId);


            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                inputDTO.CreatedBy = loginId;
                if (inputDTO.ServiceType == "RoomCharges")
                {
                    await _guestsAPIController.PostRoomChargesToAudit(inputDTO);
                }
                else
                {
                    await _guestsAPIController.PostOtherChargesToAudit(inputDTO);
                }

                //await _guestsAPIController.PostChargesToLedger(inputDTO.GuestId ?? 0);

            }

            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }
    public async Task<IActionResult> RemoveRecordFromBillingData([FromBody] BillingDTO? inputDTO)
    {
        if (inputDTO != null)
        {
            int loginId = Convert.ToInt32(User.FindFirstValue("Id"));
            var res = await _guestsAPIController.RemoveRecordFromBillingData(inputDTO);
            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }
    [Authorize]
    public async Task<IActionResult> ValidateCreditNote(string creditNoteCode, int guestId)
    {
        var res = await _guestsAPIController.ValidateCreditNote(creditNoteCode, guestId);
        return res;
    }

    [Authorize]
    public async Task<IActionResult> SavePaymentInformation([FromBody] PaymentDTO? inputDTO)
    {
        if (inputDTO != null)
        {
            int loginId = Convert.ToInt32(User.FindFirstValue("Id"));

            inputDTO.IsActive = true;
            inputDTO.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            inputDTO.CreatedBy = loginId;
            inputDTO.IsActive = true;

            var res = await _guestsAPIController.SavePaymentData(inputDTO);
            //await _guestsAPIController.PostPaymentsToLedger(inputDTO.GuestId ?? 0);

            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }


    #endregion
    #region Settlement
    public async Task<IActionResult> SettlementPartialView([FromBody] MemberDetailsWithChild? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();

        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(inputDTO?.Id ?? 0);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.MembersWithAttributes = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
            if (dto.MembersWithAttributes != null)
            {
                dto.MembersWithAttributes.GuestIdPaxSN1 = inputDTO?.GuestIdPaxSN1;
            }
        }
        var billingRes = await _guestsAPIController.GetBillingDataWithAttr(inputDTO?.Id ?? 0);
        if (billingRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)billingRes).StatusCode == 200)
        {
            dto.BillingDataList = (List<BillingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)billingRes).Value;
        }
        var paymentRes = await _guestsAPIController.GetPaymentDataWithAttr(inputDTO?.Id ?? 0);
        if (paymentRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)paymentRes).StatusCode == 200)
        {
            dto.PaymentsWithAttr = (List<PaymentWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)paymentRes).Value;
        }

        SettlementDTO settlementDTO = new SettlementDTO();
        
        // Calculate balance to determine if we need credit note or debit note
        double invoicedAmount = dto.BillingDataList?.Where(x => x.ServiceType == "GrossAmount").FirstOrDefault()?.TotalAmount ?? 0;
        double paymentCollected = dto.PaymentsWithAttr?.Sum(x => x.Amount) ?? 0;
        double balance = invoicedAmount - paymentCollected;
        
        if (balance < 0)
        {
            // Payment > Invoice: Generate credit note number
            settlementDTO.NoteNumber = await _guestsAPIController.GenerateNextCreditNoteNumberAsync();
        }
        else if (balance > 0)
        {
            // Payment < Invoice: Generate debit note number
            settlementDTO.DebitNoteNumber = await _guestsAPIController.GenerateNextDebitNoteNumberAsync();
        }
        else
        {
            // Balance = 0: Generate credit note number (default)
            settlementDTO.NoteNumber = await _guestsAPIController.GenerateNextCreditNoteNumberAsync();
        }
        
        dto.SettlementDTO = settlementDTO;

        return PartialView("_guestsList/_SettlementDrawer", dto);
    }

    [Authorize]
    public async Task<IActionResult> SaveSettlementInformation([FromBody] SettlementDTO? inputDTO)
    {
        if (inputDTO != null)
        {
            int loginId = Convert.ToInt32(User.FindFirstValue("Id"));

            inputDTO.IsActive = true;
            inputDTO.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            inputDTO.CreatedBy = loginId;
            var res = await _guestsAPIController.SaveSettlementInformation(inputDTO);
            return res;
            //return Ok();
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }

    [Route("Guests/PrintInvoice/{Id}")]
    public async Task<IActionResult> PrintInvoice(int Id)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();
        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(Id);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.MembersWithAttributes = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }
        var settlementRes = await _guestsAPIController.GetSettlementData(Id);
        if (settlementRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)settlementRes).StatusCode == 200)
        {
            dto.SettlementDTO = (SettlementDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)settlementRes).Value;
        }
        var allGuestsInRoomRes = await _guestsAPIController.GetAllGuestsInRoom(Id);
        if (allGuestsInRoomRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).StatusCode == 200)
        {
            dto.MemberDetailsWithChildren = (List<MemberDetailsWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)allGuestsInRoomRes).Value;
        }
        var billingRes = await _guestsAPIController.GetBillingDataWithAttr(Id);
        if (billingRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)billingRes).StatusCode == 200)
        {
            dto.BillingDataList = (List<BillingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)billingRes).Value;
        }
        var paymentRes = await _guestsAPIController.GetPaymentDataWithAttr(Id);
        if (paymentRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)paymentRes).StatusCode == 200)
        {
            dto.PaymentsWithAttr = (List<PaymentWithAttr>?)((Microsoft.AspNetCore.Mvc.ObjectResult)paymentRes).Value;
        }
        return View(dto);
    }

    [Route("Guests/PrintCreditNote/{Id}")]
    public async Task<IActionResult> PrintCreditNote(int Id)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();
        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(Id);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.MembersWithAttributes = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }
        var creditNoteRes = await _guestsAPIController.GetCreditNoteData(Id);
        if (creditNoteRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)creditNoteRes).StatusCode == 200)
        {
            var creditNotes = (List<CreditDebitNoteAccountDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)creditNoteRes).Value;
            if (creditNotes != null && creditNotes.Any())
            {
                dto.CreditNoteAccount = creditNotes.FirstOrDefault();
            }
        }
        return View(dto);
    }

    #endregion


    #region ChangeReservationDetails

    public async Task<IActionResult> ChangeReservationDetailsPartialView([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();
        if (inputDTO != null)
        {
            var guests = await _guestsAPIController.GetGuestById(inputDTO.Id);
            if (guests != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guests).StatusCode == 200)
            {
                dto.MemberDetail = (MembersDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)guests).Value;
            }
        }
        else
        {
            return BadRequest("No Guest Selected.Please refresh the page and try again");
        }

        var servicesRes = await _servicesAPIController.List();
        if (servicesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).StatusCode == 200)
        {
            dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).Value;
        }
        var roomTypes = await _roomTypeAPIController.List();
        if (roomTypes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).StatusCode == 200)
        {
            dto.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomTypes).Value;
        }

        return PartialView("_guestsList/_changeReservationDetails", dto);
    }
    public async Task<IActionResult> UpdateGuestReservationDetails(GuestsListViewModel dataVM)
    {
        if (dataVM != null && dataVM.MemberDetail != null)
        {
            var res = await _guestsAPIController.UpdateGuestReservationDetails(dataVM);
            return res;
        }
        else
        {
            return BadRequest("The guest details are invalid;");
        }
        return BadRequest("");
    }


    #endregion


    #region Add Services
    public async Task<IActionResult> AddServicesPartialView([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();

        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(inputDTO?.Id ?? 0);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.MembersWithAttributes = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }

        var servicesRes = await _guestsAPIController.GetTasks();
        if (servicesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).StatusCode == 200)
        {
            dto.ServicesForAddServices = (List<TaskMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).Value;
        }


        return PartialView("_guestsList/_addServices", dto);
    }
    #endregion


    #region ChangeInOutDates
    public async Task<IActionResult> ChangeInOutDatePartialView([FromBody] MembersDetailsDTO? inputDTO)
    {
        GuestsListViewModel? dto = new GuestsListViewModel();

        var guestRes = await _guestsAPIController.GetGuestByIdWithChild(inputDTO?.Id ?? 0);
        if (guestRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).StatusCode == 200)
        {
            dto.MembersWithAttributes = (MemberDetailsWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)guestRes).Value;
        }


        var roomAllocation = await _guestsAPIController.GetRoomAllocationByGuestId(inputDTO?.Id ?? 0);
        if (roomAllocation != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomAllocation).StatusCode == 200)
        {
            dto.RoomAllocation = (RoomAllocationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomAllocation).Value;
        }


        return PartialView("_guestsList/_changeInOutDate", dto);
    }

    public async Task<IActionResult> SaveCheckInOutDates([FromBody] RoomAllocationDTO? inputDTO)
    {
        var res = await _guestsAPIController.UpdateCheckInOutDates(inputDTO);

        return res;
    }

    #endregion
}
