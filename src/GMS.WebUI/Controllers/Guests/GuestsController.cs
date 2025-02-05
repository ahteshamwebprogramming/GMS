using GMS.Core.Entities;
using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Guests;
using Microsoft.AspNetCore.Authorization;
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
        var servicesRes = await _servicesAPIController.ServicesList();
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
        var channelCodes = await _channelCodeAPIController.ChannelCodeList();
        if (channelCodes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)channelCodes).StatusCode == 200)
        {
            viewModel.ChannelCodes = (List<ChannelCodeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)channelCodes).Value;
        }
        var guarenteeCodes = await _guaranteeCodeAPIController.GuaranteeCodeList();
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
                inputDTO.City = dataVM.MemberDetail.City;
                inputDTO.State = dataVM.MemberDetail.State;
                inputDTO.Country = dataVM.MemberDetail.Country;
                inputDTO.Pincode = dataVM.MemberDetail.Pincode;
                inputDTO.Idproof = dataVM.MemberDetail.Idproof;
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
                inputDTO.PassportNo = dataVM.MemberDetail.PassportNo;
                inputDTO.PolicyHolder = dataVM.MemberDetail.PolicyHolder;
                inputDTO.InsuranceCompany = dataVM.MemberDetail.InsuranceCompany;
                inputDTO.PolicyNo = dataVM.MemberDetail.PolicyNo;

                inputDTO.RoomType = dataVM.MemberDetail.RoomType;
                inputDTO.HealthIssues = dataVM.MemberDetail.HealthIssues;

                inputDTO.IsMonthlynewsletter = dataVM.MemberDetail.IsMonthlynewsletter;

                inputDTO.BloodGroup = dataVM.MemberDetail.BloodGroup;
                inputDTO.VisaDetails = dataVM.MemberDetail.VisaDetails;
                inputDTO.CoveredIndia = dataVM.MemberDetail.CoveredIndia;
                inputDTO.Staydays = 0;

                inputDTO.Nights = "";
                inputDTO.Nationality = dataVM.MemberDetail.Nationality;
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

                        inputDTO.GroupId = await _guestsAPIController.GenerateRandomNumberAndValidate();//When Id=0
                    }
                }
                else if (String.IsNullOrEmpty(dataVM.MemberDetail.UniqueNo))
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
                    }
                }
                if (String.IsNullOrEmpty(inputDTO.UniqueNo))
                {
                    inputDTO.UniqueNo = "NAAD00" + CommonHelper.GenerateNaadRandomNo();//When Id=0
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

                #region Photo Attachments

                try
                {
                    var attachments = dataVM.PhotoAttachment;
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            if (attachment.Length > 0)
                            {
                                var attachmentName = Path.GetFileName(attachment.FileName);
                                var attachmentExtension = Path.GetExtension(attachmentName);
                                string fileName = inputDTO.UniqueNo + attachmentExtension;
                                string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "MembersPic");
                                string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                if (!Directory.Exists(FilePath))
                                    Directory.CreateDirectory(FilePath);
                                var filePath = Path.Combine(FilePath, fileName);
                                inputDTO.Photo = fileName;
                                using (FileStream fs = System.IO.File.Create(filePath))
                                {
                                    attachment.CopyTo(fs);
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
                    var attachments = dataVM.IdProofAttachment;
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            if (attachment.Length > 0)
                            {
                                var attachmentName = Path.GetFileName(attachment.FileName);
                                var attachmentExtension = Path.GetExtension(attachmentName);
                                string fileName = inputDTO.UniqueNo + attachmentExtension;
                                string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "IDProof");
                                string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                if (!Directory.Exists(FilePath))
                                    Directory.CreateDirectory(FilePath);
                                var filePath = Path.Combine(FilePath, fileName);
                                using (FileStream fs = System.IO.File.Create(filePath))
                                {
                                    attachment.CopyTo(fs);
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
                    var attachments = dataVM.PassportAttachment;
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            if (attachment.Length > 0)
                            {
                                var attachmentName = Path.GetFileName(attachment.FileName);
                                var attachmentExtension = Path.GetExtension(attachmentName);
                                string fileName = inputDTO.UniqueNo + attachmentExtension;
                                string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "VisaDetails");
                                string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                if (!Directory.Exists(FilePath))
                                    Directory.CreateDirectory(FilePath);
                                var filePath = Path.Combine(FilePath, fileName);
                                using (FileStream fs = System.IO.File.Create(filePath))
                                {
                                    attachment.CopyTo(fs);
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
                var res = await _guestsAPIController.ManageGuests(inputDTO);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    if (inputDTO.PAXSno == inputDTO.Pax)
                    {
                        inputDTO.PaxCompleted = true;
                        await _guestsAPIController.UpdatePAxCompleted(inputDTO);
                    }


                    if (dataVM.Source == "RoomAvailability")
                    {
                        RoomAllocationDTO roomAllocationDTO = new RoomAllocationDTO();
                        roomAllocationDTO.Rnumber = dataVM?.RoomAllocation?.Rnumber;
                        roomAllocationDTO.Rtype = inputDTO.RoomType;
                        roomAllocationDTO.GuestId = inputDTO.Id;
                        roomAllocationDTO.Fd = inputDTO?.DateOfArrival?.ToString("MM/dd/yyyy hh:mm tt");
                        roomAllocationDTO.Td = inputDTO?.DateOfDepartment?.ToString("MM/dd/yyyy hh:mm tt");
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
    public async Task<IActionResult> AddRoomPartialView([FromBody] MembersDetailsDTO inputDTO)
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
    public async Task<IActionResult> AllocateRoom([FromBody] RoomAllocationDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _guestsAPIController.AllocateRoom(inputDTO);
            return res;
        }
        return BadRequest("Unable to assign room at the moment");
    }

    public async Task<IActionResult> CheckInGuest([FromBody] MedicalSoultion_GuestCheckList inputDTO)
    {
        var res = await _guestsAPIController.GuestCheckIn(inputDTO);
        return res;
    }

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
}
