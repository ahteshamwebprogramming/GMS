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
using Rooms.Controllers;

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
    private readonly RoomRateAPIController _roomRateAPIController;
    private readonly BulkRateAPIController _bulkRateAPIController;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public RoomsController(ILogger<RoomsController> logger, RoomTypeAPIController roomTypeAPIController, RoomsAvailabilityAPIController roomsAvailabilityAPIController, RoomLockingAPIController roomLockingAPIController, GuestsAPIController guestsAPIController, GenderAPIController genderAPIController, CountryAPIController countryAPIController, ServicesAPIController servicesAPIController, CarTypeAPIController carTypeAPIController, BrandAwarenessAPIController brandAwarenessAPIController, LeadSourceAPIController leadSourceAPIController, ChannelCodeAPIController channelCodeAPIController, GuaranteeCodeAPIController guaranteeCodeAPIController, IWebHostEnvironment hostingEnv, RoomRateAPIController roomRateAPIController, BulkRateAPIController bulkRateAPIController)
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
        _roomRateAPIController = roomRateAPIController;
        _bulkRateAPIController = bulkRateAPIController;
    }
    public async Task<IActionResult> RoomsAvailabality()
    {
        return View();
    }
    public async Task<IActionResult> RoomsDetails()
    {
        RoomStatusViewModel dto = new RoomStatusViewModel();
        var res = await _roomLockingAPIController.GetRooms();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.Rooms = (List<RoomsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return View(dto);
    }
    public async Task<IActionResult> RoomsReadyStatusDetails()
    {
        RoomStatusViewModel dto = new RoomStatusViewModel();
        var res = await _roomLockingAPIController.GetRooms();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.Rooms = (List<RoomsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        return View(dto);
    }

    #region RoomRates

    public async Task<IActionResult> RoomRates(DateTime? startDate, int PlanId)
    {
        var rooms = await _roomRateAPIController.GetAllAsync();
        var date = startDate ?? DateTime.Today;
        var roomRates = await _roomRateAPIController.GetRoomRatesAsync(date, 10, PlanId);
        var (roomInventory, TotalRooms, OccupancyPercentage) = await _roomRateAPIController.GetRoomInventoryWithOccupancy(date, 10);
        ViewBag.RoomInventory = roomInventory;
        ViewBag.TotalRooms = TotalRooms;
        ViewBag.occupency = OccupancyPercentage.ToString("F2");

        var servicesRes = await _servicesAPIController.List();
        if (servicesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).StatusCode == 200)
        {
            ViewBag.RatePlans = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).Value;
        }
        ViewBag.PlanId = PlanId;
        return View(roomRates);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateRates(RatesUpdateViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var success = await _roomRateAPIController.UpdateRatesAsync(model);
                if (success)
                {
                    return Json(new { success = true, message = "Inventory updated successfully" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating inventory: {ex.Message}" });

            }
        }

        var Date = model.Rates.First().Date < DateTime.Today ? DateTime.Today : model.Rates.First().Date;
        var RoomRates = await _roomRateAPIController.GetRoomRatesAsync(Date, 10, 1034);
        ViewBag.StartDate = Date;
        return PartialView("_RatesTable", RoomRates);
    }


    [HttpGet]
    public async Task<IActionResult> GetRatesTable(DateTime? startDate, int PlanId)
    {
        var date = startDate ?? DateTime.Today;
        if (date < DateTime.Today)
        {
            date = DateTime.Today;
        }
        var roomRates = await _roomRateAPIController.GetRoomRatesAsync(date, 10, PlanId);
        //var roomInventory = await _roomRepository.GetRoomInventory(date, 10);
        ViewBag.StartDate = date;
        var (roomInventory, TotalRooms, OccupancyPercentage) = await _roomRateAPIController.GetRoomInventoryWithOccupancy(date, 10);
        ViewBag.RoomInventory = roomInventory;
        ViewBag.TotalRooms = TotalRooms;
        ViewBag.occupency = OccupancyPercentage.ToString("F2");

        var servicesRes = await _servicesAPIController.List();
        if (servicesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).StatusCode == 200)
        {
            ViewBag.RatePlans = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).Value;
        }
        ViewBag.PlanId = PlanId;
        return PartialView("_roomRates/_ratesTable", roomRates);
    }


    #endregion

    #region Bulk Rate
    public async Task<IActionResult> BulkRate(DateTime? fromDate, DateTime? toDate)
    {
        var startDate = fromDate ?? DateTime.Today;
        if (startDate < DateTime.Today)
        {
            startDate = DateTime.Today;
        }
        var endDate = toDate ?? startDate.AddDays(9);
        if (endDate < startDate)
        {
            endDate = startDate;
        }
        if ((endDate - startDate).Days > 364)
        {
            endDate = startDate.AddDays(364);
        }

        var days = 1;
        List<ServicesDTO>? RatePlansList = new List<ServicesDTO>();
        //var ratePlans = await _roomRateAPIController.GetAllRatePlans();
        //if (ratePlans != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)ratePlans).StatusCode == 200)
        //{
        //    RatePlansList = (List<RatePlansDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)ratePlans).Value;
        //}
        var servicesRes = await _servicesAPIController.List();
        if (servicesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).StatusCode == 200)
        {
            RatePlansList = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)servicesRes).Value;
        }
        // var roomRates = await _roomRepository.GetRoomRatesAsync(startDate, days);
        //var roomInventory = await _roomRepository.GetRoomInventory(startDate, days);
        //var roomRestrictions = await _roomRepository.GetRoomRestrictionsAsync(startDate, days);
        //var model = new BulkUpdateViewModel
        //{
        //    FromDate = startDate,
        //    ToDate = endDate,
        //    Rates = roomRates.Select(r => new RoomRateBulkUpdate
        //    {
        //        RoomTypeId = r.RoomTypeId,
        //        RoomTypeName = r.RoomTypeName,
        //        SaleRate = r.DailyRates.FirstOrDefault()?.Price,
        //        MinimumRate = r.DailyRates.FirstOrDefault()?.MinRate,
        //        MaximumRate = r.DailyRates.FirstOrDefault()?.MaxRate
        //    }).ToList()
        //};
        var model = new BulkUpdateViewModel
        {
            FromDate = startDate,
            ToDate = endDate,
            // Only initialize empty lists; data will be fetched via AJAX
            Rates = new List<RoomRateBulkUpdate>(),
            Inventory = new List<RoomInventoryBulkUpdate>(),
            Restrictions = new List<RoomRestrictionBulkUpdate>(),
            RatePlansList = RatePlansList
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryData(DateTime fromDate, DateTime toDate, int planId)
    {
        var days = (toDate - fromDate).Days + 1;
        var roomInventory = await _bulkRateAPIController.GetRoomInventory(fromDate, days, planId);
        var inventory = roomInventory.Select(r => new RoomInventoryBulkUpdate
        {
            RoomTypeId = r.RtypeID ?? 0,
            RoomTypeName = r.RoomTypeName,
            RoomsAvailable = r.DailyInventory.FirstOrDefault()?.RoomsAvailable,
            RoomsOpen = r.DailyInventory.FirstOrDefault()?.TotalRoomForSale
        }).ToList();
        return Json(inventory);
    }
    [HttpGet]
    public async Task<IActionResult> GetRatesData(DateTime fromDate, DateTime toDate)
    {
        var days = (toDate - fromDate).Days + 1;
        var roomRates = await _bulkRateAPIController.GetRoomRatesAsync(fromDate, days);
        var rates = roomRates.Select(r => new RoomRateBulkUpdate
        {
            RoomTypeId = r.RoomTypeId ?? 0,
            RoomTypeName = r.RoomTypeName,
            SaleRate = r.DailyRates.FirstOrDefault()?.Price,
            MinimumRate = r.DailyRates.FirstOrDefault()?.MinRate,
            MaximumRate = r.DailyRates.FirstOrDefault()?.MaxRate
        }).ToList();
        return Json(rates);
    }

    [HttpGet]
    public async Task<IActionResult> GetRatesData_New(DateTime fromDate, DateTime toDate, int planId, string? selectedDays)
    {
        var days = (toDate - fromDate).Days + 1;
        var roomRates = await _bulkRateAPIController.GetRoomRatesAsync(fromDate, days, planId);
        var rates = roomRates.Select(r => new RoomRateBulkUpdate
        {
            RoomTypeId = r.RoomTypeId ?? 0,
            RoomTypeName = r.RoomTypeName,
            SaleRate = r.DailyRates.FirstOrDefault()?.Price,
            MinimumRate = r.DailyRates.FirstOrDefault()?.MinRate,
            MaximumRate = r.DailyRates.FirstOrDefault()?.MaxRate
        }).ToList();
        return Json(rates);
    }
    [HttpGet]
    public async Task<IActionResult> GetIncrementData(DateTime fromDate, DateTime toDate)
    {
        var days = (toDate - fromDate).Days + 1;
        if (days > 365)
        {
            days = 365;
            toDate = fromDate.AddDays(364);
        }

        var roomData = await _bulkRateAPIController.GetRoomIncrementDataAsync(fromDate, days, fromDate, toDate);
        return Ok(roomData);
    }
    [HttpGet]
    public async Task<IActionResult> GetRestrictionsData(DateTime fromDate, DateTime toDate)
    {
        var days = (toDate - fromDate).Days + 1;
        var roomRestrictions = await _bulkRateAPIController.GetRoomRestrictionsAsync(fromDate, days);
        var restrictions = roomRestrictions.Select(r => new RoomRestrictionBulkUpdate
        {
            RoomTypeId = r.RoomTypeId,
            RoomTypeName = r.RoomTypeName,
            StopSell = r.DailyRestrictions.FirstOrDefault()?.StopSell,
            CloseOnArrival = r.DailyRestrictions.FirstOrDefault()?.CloseOnArrival,
            RestrictStay = r.DailyRestrictions.FirstOrDefault()?.RestrictStay,
            MinimumNights = r.DailyRestrictions.FirstOrDefault()?.MinimumNights,
            MaximumNights = r.DailyRestrictions.FirstOrDefault()?.MaximumNights
        }).ToList();
        return Json(restrictions);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateBulk(BulkUpdateViewModel model, string activeTab)
    {
        try
        {
            if (activeTab == "tab2")
            {
                var selectedInventory = model.Inventory.Where(i => i.IsSelected).ToList();
                if (selectedInventory.Any())
                {
                    if (!selectedInventory.All(i => i.RoomsOpen.HasValue && i.RoomsOpen >= 0))
                    {
                        return Json(new { success = false, message = "Selected inventory contains invalid RoomsOpen values" });
                    }
                    //await _bulkRateAPIController.UpdateBulkInventoryAsync(model.ChannelId, model.FromDate, model.ToDate, selectedInventory);
                    await _bulkRateAPIController.UpdateBulkInventoryAsync(model);
                    return Json(new { success = true, message = "Selected inventory updated successfully" });
                }
                return Json(new { success = false, message = "No inventory items selected for update" });
            }
            else if (activeTab == "tab1")
            {
                var selectedRates = model.Rates.Where(r => r.IsSelected).ToList();
                if (selectedRates.Any())
                {
                    if (!selectedRates.All(r => (r.MinimumRate.HasValue && r.MinimumRate >= 0) &&
                        (r.MaximumRate == null || r.MaximumRate >= 0) &&
                        (r.SaleRate.HasValue && r.SaleRate >= 0 && r.SaleRate >= r.MinimumRate &&
                        (r.MaximumRate == null || r.SaleRate <= r.MaximumRate))))
                    {
                        return Json(new { success = false, message = "Selected rates contain invalid values" });
                    }
                    await _bulkRateAPIController.UpdateBulkRatesAsync(model.ChannelId, model.FromDate, model.ToDate, selectedRates);
                    return Json(new { success = true, message = "Selected rates updated successfully" });
                }
                return Json(new { success = false, message = "No rates selected for update" });
            }

            else if (activeTab == "tab_PR")
            {
                var selectedRates = model.Rates.Where(r => r.IsSelected).ToList();
                if (selectedRates.Any())
                {
                    if (!selectedRates.All(r => (r.MinimumRate.HasValue && r.MinimumRate >= 0) &&
                        (r.MaximumRate == null || r.MaximumRate >= 0) &&
                        (r.SaleRate.HasValue && r.SaleRate >= 0 && r.SaleRate >= r.MinimumRate &&
                        (r.MaximumRate == null || r.SaleRate <= r.MaximumRate))))
                    {
                        return Json(new { success = false, message = "Selected rates contain invalid values" });
                    }
                    await _bulkRateAPIController.UpdateBulkRatesAsync_Packages(model.ChannelId, model.FromDate, model.ToDate, selectedRates, model.SelectedDaysList ?? "");
                    return Json(new { success = true, message = "Selected rates updated successfully" });
                }
                return Json(new { success = false, message = "No rates selected for update" });
            }
            else if (activeTab == "tab3")
            {

                var selectedPercentages = model.Percentages.Where(p => p.IsSelected).ToList();
                if (!selectedPercentages.Any())
                {
                    return Json(new { success = false, message = "No percentages selected for update" });
                }

                // Validate percentages
                foreach (var item in selectedPercentages)
                {
                    if (item.Percentages == null || item.Percentages.Length != 8)
                    {
                        return Json(new { success = false, message = "Invalid percentage data for room type " + item.RoomTypeId });
                    }
                }


                var percentageData = selectedPercentages.Select(p => new RoomPercentageData
                {
                    RoomTypeId = p.RoomTypeId,
                    Percentages = p.Percentages.Select(pct => pct / 100m).ToArray() // Convert to decimal
                }).ToList();

                await _bulkRateAPIController.SaveRoomPercentagesAsync(percentageData, model.FromDate, model.ToDate);
                return Json(new { success = true, message = "Selected percentages saved successfully" });

            }
            else if (activeTab == "tab4")
            {
                var selectedRestrictions = model.Restrictions.Where(r => r.IsSelected).ToList();
                if (selectedRestrictions.Any())
                {
                    if (!selectedRestrictions.All(r => (r.MinimumNights == null || r.MinimumNights >= 0) &&
                        (r.MaximumNights == null || r.MaximumNights >= 0)))
                    {
                        return Json(new { success = false, message = "Selected restrictions contain invalid values" });
                    }
                    await _bulkRateAPIController.UpdateBulkRestrictionsAsync(model.ChannelId, model.FromDate, model.ToDate, selectedRestrictions);
                    return Json(new { success = true, message = "Selected restrictions updated successfully" });
                }
                return Json(new { success = false, message = "No restrictions selected for update" });
            }
            return Json(new { success = false, message = "Invalid tab selection" });

        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error updating data: {ex.Message}" });
        }

    }

    #endregion

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
    public async Task<IActionResult> RoomDetailsListPartialView()
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
        return PartialView("_roomDetails/_list", dto);
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


    #region RoomStatus
    public async Task<IActionResult> RoomStatusRecordPartialView([FromBody] RoomsDTO? inputDTO)
    {
        RoomStatusViewModel? dto = new RoomStatusViewModel();
        var res = await _roomLockingAPIController.GetRoomWithCleaningAttributeById(inputDTO?.Id ?? 0);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            if (inputDTO?.Id == 0)
            {
                dto.RoomWithAttributeList = (List<RoomsWithAttribute>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                return PartialView("_roomReadyStatus/_list", dto);
            }
            else
            {
                dto.RoomWithAttribute = (RoomsWithAttribute?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                return PartialView("_roomDetails/_list", dto);
            }
        }
        else
        {
            return PartialView("_roomDetails/_list", dto);
        }
    }
    public async Task<IActionResult> RoomCleanCheckList([FromBody] RoomsDTO? inputDTO)
    {
        RoomStatusViewModel? dto = new RoomStatusViewModel();
        var res = await _roomLockingAPIController.GetRoomCleanCheckList();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.RoomCleanCheckList = (List<TblCheckListsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        }
        RoomChkListDTO roomChkListDTO = new RoomChkListDTO();
        roomChkListDTO.RID = inputDTO?.Id ?? 0;
        roomChkListDTO.CheckedByName = User.FindFirstValue(ClaimTypes.Name);
        roomChkListDTO.ChkDate = DateTime.Now;
        dto.RoomCheckList = roomChkListDTO;

        return PartialView("_roomDetails/_roomCleanList", dto);
    }
    public async Task<IActionResult> CleanRoomCheck([FromBody] RoomChkListDTO inputDTO)
    {
        if (inputDTO != null)
        {
            inputDTO.CheckedBy = Convert.ToInt32(User.FindFirstValue("Id"));
            inputDTO.ChkDate = DateTime.Now;
            var res = await _roomLockingAPIController.RoomCleanCheckList(inputDTO);
            return res;
        }
        return BadRequest("Invalid Data");
    }
    #endregion


    public async Task<IActionResult> CopyRoomRateBulk([FromBody] BulkCopyDTO inputDTO)
    {
        if (inputDTO != null)
        {
            var res = await _roomRateAPIController.CopyBulkRates(inputDTO);
            return res;
        }
        return BadRequest("Data is invalid");
    }

}
