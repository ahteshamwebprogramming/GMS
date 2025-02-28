using GMS.Endpoints.Accounts;
using GMS.Endpoints.Guests;
using GMS.Endpoints.Masters;
using GMS.Endpoints;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.Rooms;
using GMS.WebUI.Controllers.Guests;
using Microsoft.AspNetCore.Mvc;
using GMS.Endpoints.Rooms;
using GMS.Infrastructure.Models.Rooms;
using System.Security.Claims;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Core.Entities;
using Microsoft.AspNetCore.Components.Forms;
using GMS.Infrastructure.Helper;
using Humanizer;

namespace GMS.WebUI.Controllers.Rooms;

public class RoomsController : Controller
{
    private readonly ILogger<RoomsController> _logger;
    private readonly RoomTypeAPIController _roomTypeAPIController;
    private readonly RoomsAvailabilityAPIController _roomsAvailabilityAPIController;
    private readonly RoomLockingAPIController _roomLockingAPIController;
    private readonly GuestsAPIController _guestsAPIController;
    private readonly GenderAPIController _genderAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly ServicesAPIController _servicesAPIController;
    private readonly CarTypeAPIController _carTypeAPIController;
    private readonly BrandAwarenessAPIController _brandAwarenessAPIController;
    private readonly LeadSourceAPIController _leadSourceAPIController;
    private readonly ChannelCodeAPIController _channelCodeAPIController;
    private readonly GuaranteeCodeAPIController _guaranteeCodeAPIController;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public RoomsController(ILogger<RoomsController> logger, RoomTypeAPIController roomTypeAPIController, RoomsAvailabilityAPIController roomsAvailabilityAPIController, RoomLockingAPIController roomLockingAPIController, GuestsAPIController guestsAPIController, GenderAPIController genderAPIController, CountryAPIController countryAPIController, ServicesAPIController servicesAPIController, CarTypeAPIController carTypeAPIController, BrandAwarenessAPIController brandAwarenessAPIController, LeadSourceAPIController leadSourceAPIController, ChannelCodeAPIController channelCodeAPIController, GuaranteeCodeAPIController guaranteeCodeAPIController, IWebHostEnvironment hostingEnv)
    {
        _logger = logger;
        _roomTypeAPIController = roomTypeAPIController;
        _roomsAvailabilityAPIController = roomsAvailabilityAPIController;
        _roomLockingAPIController = roomLockingAPIController;
        _guestsAPIController = guestsAPIController;
        _genderAPIController = genderAPIController;
        _countryAPIController = countryAPIController;
        _servicesAPIController = servicesAPIController;
        _carTypeAPIController = carTypeAPIController;
        _brandAwarenessAPIController = brandAwarenessAPIController;
        _leadSourceAPIController = leadSourceAPIController;
        _channelCodeAPIController = channelCodeAPIController;
        _guaranteeCodeAPIController = guaranteeCodeAPIController;
        _hostingEnv = hostingEnv;
    }
    public async Task<IActionResult> RoomsAvailabality()
    {
        return View();
    }
    public async Task<IActionResult> RoomsDetails()
    {
        return View();
    }
    public async Task<IActionResult> TodayBooking()
    {
        return View();
    }
    public async Task<IActionResult> BookingList()
    {
        return View();
    }

    public async Task<IActionResult> RoomLocking()
    {
        return View();
    }
    public async Task<IActionResult> RoomStatus()
    {
        return View();
    }

    public async Task<IActionResult> MasterScheduleListPartialView([FromBody] RoomsAvailabilityViewModel? dates)
    {
        RoomsAvailabilityViewModel? dto = new RoomsAvailabilityViewModel();
        var res = await _roomsAvailabilityAPIController.GetRoomsAvailabilityList(dates);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.RoomAvailabilities = (List<RoomAvailabilityDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }

        return PartialView("_roomsAvailability/_listRoomAvailability", dto);
    }
    public async Task<IActionResult> RoomLockingAddPartialView([FromBody] RoomLockDAO? inputDTO)
    {
        RoomLockingViewModel? dto = new RoomLockingViewModel();
        var res = await _roomLockingAPIController.GetRooms();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.RoomsList = (List<RoomsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        dto.RoomLockDAO = inputDTO;

        return PartialView("_roomLocking/_add", dto);
    }
    public async Task<IActionResult> AddNewRoomPartialView([FromBody] RoomsDTO inputDTO)
    {
        RoomLockingViewModel? dto = new RoomLockingViewModel();
        if (inputDTO != null && inputDTO.Id > 0)
        {
            var roomRes = await _roomLockingAPIController.GetRoomById(inputDTO.Id);
            if (roomRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomRes).StatusCode == 200)
            {
                dto.Room = (RoomsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomRes).Value;

                if (dto.Room != null)
                {
                    var roomAmenetiesRes = await _roomLockingAPIController.GetRoomAmenetiesByRoomNumber(dto.Room.Rnumber ?? "");
                    if (roomAmenetiesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)roomAmenetiesRes).StatusCode == 200)
                    {
                        dto.RoomAmenitiesAssigned = (List<RoomAmenetiesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)roomAmenetiesRes).Value;

                        dto.AmenityIds = dto?.RoomAmenitiesAssigned?.Select(amenity => amenity.AmenityId ?? 0).ToArray();
                    }
                }
            }
        }

        var res = await _roomLockingAPIController.GetRoomTypes();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.RoomTypes = (List<RoomTypeDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        var resAmeneties = await _roomLockingAPIController.GetRoomAmeneties();
        if (resAmeneties != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAmeneties).StatusCode == 200)
        {
            dto.Amenities = (List<AmenitiesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAmeneties).Value;
        }
        return PartialView("_roomLocking/_addNewRoom", dto);
    }

    
    public async Task<IActionResult> ViewRoomAmenetiesPartialView([FromBody] RoomsDTO inputDTO)
    {
        RoomLockingViewModel? dto = new RoomLockingViewModel();
        if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.Rnumber))
        {
            var res = await _roomLockingAPIController.GetRoomAmenetiesWithChildByRoomNumber(inputDTO.Rnumber);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomAmenitiesAssignedWithChild = (List<RoomAmenetiesWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_roomLocking/_viewRoomAmeneties", dto);
    }
    public async Task<IActionResult> ViewRoomImagesPartialView([FromBody] RoomsDTO inputDTO)
    {
        RoomLockingViewModel? dto = new RoomLockingViewModel();
        if (inputDTO != null && inputDTO.Id > 0)
        {
            var res = await _roomLockingAPIController.GetRoomImagesByRoomNumber(inputDTO.Id);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomImages = (List<RoomsPicturesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_roomLocking/_viewRoomImages", dto);
    }
    public async Task<IActionResult> RoomLockingListPartialView()
    {
        RoomLockingViewModel? dto = new RoomLockingViewModel();
        var res1 = await _roomLockingAPIController.GetRoomLockingList();
        if (res1 != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res1).StatusCode == 200)
        {
            dto.RoomLockList = (List<RoomLockDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res1).Value;
        }
        var res = await _roomLockingAPIController.GetRoomsWithLockAndHoldStatus();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.RoomsWithStatuses = (List<RoomsWithStatusDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return PartialView("_roomLocking/_list", dto);
    }
    public async Task<IActionResult> SaveRoomLockingDetails([FromBody] RoomLockDTO? inputDTO)
    {
        if (inputDTO != null)
        {
            if (inputDTO.Id == 0)
            {
                inputDTO.Status = 1;
                inputDTO.CrDate = DateTime.Now;
            }
            var res = await _roomLockingAPIController.SaveRoomLockingDetails(inputDTO);
            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
    }
    public async Task<IActionResult> UnHold([FromBody] RoomLockDAO? inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _roomLockingAPIController.UnHold(inputDTO);
            return res;
        }
        else
        {
            return BadRequest("Data is not valid");
        }
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
        if (viewModel != null && viewModel.MemberDetail != null)
            viewModel.MemberDetail.DateOfArrival = DateTime.Now.Date.AddHours(12);
        else if (viewModel != null && viewModel.MemberDetail == null)
        {
            viewModel.MemberDetail = new MembersDetailsDTO();
            viewModel.MemberDetail.DateOfArrival = DateTime.Now.Date.AddHours(12);
        }
        return PartialView("_roomsAvailability/_addGuests", viewModel);
    }

    public async Task<IActionResult> ViewGuestsInRoom([FromBody] RoomAvailabilityDTO inputDTO)
    {
        RoomsAvailabilityViewModel? dto = new RoomsAvailabilityViewModel();
        dto.RoomNo = inputDTO.RNumber;
        if (inputDTO != null && inputDTO.RNumber != null && inputDTO.DateValue != null)
        {
            var res = await _guestsAPIController.GetGuestsDetailsInRooms(inputDTO.RNumber, inputDTO.DateValue ?? default(DateTime));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.MembersDetails = (List<MembersDetailsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                if (dto.MembersDetails != null && dto.MembersDetails.Count > 0)
                {
                    dto.GroupId = dto.MembersDetails[0].GroupId;
                }
            }
        }
        return PartialView("_roomsAvailability/_guestsDetails", dto);
    }

    [HttpPost]
    public async Task<IActionResult> SaveRoom(RoomLockingViewModel dataVM)
    {

        try
        {
            if (dataVM != null && dataVM.Room != null)
            {
                #region Save RoomData
                var resRooms = await _roomLockingAPIController.SaveRoom(dataVM.Room);
                #endregion

                #region Save Ameneties
                if (resRooms != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRooms).StatusCode == 200)
                {
                    RoomsDTO? rooms = (RoomsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRooms).Value;
                    if (rooms != null && dataVM.AmenityIds != null)
                    {
                        List<RoomAmenetiesDTO> roomAmeneties = new List<RoomAmenetiesDTO>();
                        foreach (var item in dataVM.AmenityIds)
                        {
                            roomAmeneties.Add(new RoomAmenetiesDTO { RoomNumber = rooms.Rnumber, AmenityId = item });
                        }
                        await _roomLockingAPIController.AddAmeneties(roomAmeneties);
                    }
                }
                #endregion

                #region Photo Attachments
                try
                {
                    if (resRooms != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRooms).StatusCode == 200)
                    {
                        RoomsDTO? rooms = (RoomsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRooms).Value;
                        if (rooms != null)
                        {
                            var attachments = dataVM.Attachments;
                            if (attachments != null)
                            {
                                foreach (var attachment in attachments)
                                {
                                    if (attachment.Length > 0)
                                    {
                                        var attachmentName = Path.GetFileNameWithoutExtension(attachment.FileName);
                                        var attachmentExtension = Path.GetExtension(attachment.FileName);
                                        string fileName = attachmentName + attachmentExtension;
                                        string FilePathWithoutRoot = Path.Combine("UploadedDocuments", "RoomsPicture", rooms.Id.ToString());
                                        string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);

                                        RoomsPicturesDTO roomsPicturesDTO = new RoomsPicturesDTO();
                                        roomsPicturesDTO.AttachmentName = attachmentName;
                                        roomsPicturesDTO.AttachmentExtension = attachmentExtension;
                                        roomsPicturesDTO.RoomPicturePath = FilePathWithoutRoot;
                                        roomsPicturesDTO.RoomId = rooms.Id;
                                        roomsPicturesDTO.IsActive = true;

                                        var resRP = await _roomLockingAPIController.CheckAndAddPictureToDatabase(roomsPicturesDTO);
                                        if (resRP != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resRP).StatusCode == 200)
                                        {
                                            RoomsPicturesDTO? insertedRoomsPicturesDTO = (RoomsPicturesDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resRP).Value;
                                            if (insertedRoomsPicturesDTO != null)
                                            {
                                                if (!Directory.Exists(FilePath))
                                                    Directory.CreateDirectory(FilePath);
                                                var filePath = Path.Combine(FilePath, insertedRoomsPicturesDTO.AttachmentName + attachmentExtension);
                                                //inputDTO.Photo = fileName;
                                                using (FileStream fs = System.IO.File.Create(filePath))
                                                {
                                                    attachment.CopyTo(fs);
                                                }
                                            }
                                        }
                                    }
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

                return resRooms;
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
    public async Task<IActionResult> DeleteRoom([FromBody] RoomsDTO inputDTO)
    {
        if (inputDTO.Id > 0)
        {
            var res = await _roomLockingAPIController.DeleteRoom(inputDTO.Id);
            return res;
        }
        return BadRequest("Unable to delete right now");
    }
}
