using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.Masters;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GMS.Endpoints.Guests;

[Route("api/[controller]")]
[ApiController]
public class GuestsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GuestsAPIController> _logger;
    private readonly IMapper _mapper;
    public GuestsAPIController(IUnitOfWork unitOfWork, ILogger<GuestsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> GetGuestsList()
    {
        try
        {
            //string query = "Select * from GMSFinalGuest order by id desc";
            string query = "Select md.PaxCompleted,md.Id\r\n,UniqueNo\r\n,CustomerName\r\n,MobileNo\r\n,s.Service\r\n,mc.Category\r\n,md.Photo\r\n,(Case when IsApproved=1 then 'Approved' else 'Awating' end) IsApproved\r\n,md.CreationDate\r\n,md.Status\r\n,g.Gender\r\n,(select count(1) from [dbo].[GuestsChkList] gcl where gcl.GuestID=md.Id) IsChecked\r\n,md.Age\r\n,md.Nationality\r\n,(CASE WHEN DateOfArrival IS NULL OR DateOfArrival ='' THEN '' ELSE convert(nvarchar,DateOfArrival,106)END ) DateOfArrival\r\n,(CASE WHEN DateOfDepartment IS NULL OR DateOfDepartment ='' THEN '' ELSE convert(nvarchar,DateOfDepartment,106)END )DateOfDeparture\r\n,rt.RType\r\n,(SELECT STUFF((SELECT ', ' + convert(nvarchar(20),'Room No '+ra.RNumber) \r\n               FROM RoomAllocation ra \r\n               WHERE ra.GuestID = md.Id \r\n                 AND ra.FD = md.DateOfArrival \r\n                 AND ra.TD = md.DateOfDepartment \r\n                 AND ra.IsActive = 1 \r\n               FOR XML PATH('')), 1, 2, '')) AS RoomNumber\r\n\r\nfrom MembersDetails md\r\nleft Join RoomType rt on md.RoomType=rt.ID\r\nleft Join GenderMaster g on md.Gender=g.Id\r\nleft join MstCategory mc on mc.ID=md.ServiceId\r\nleft join Services s on s.ID=md.CatID\r\nwhere md.Status=1\r\norder by md.Id asc";
            var res = await _unitOfWork.GMSFinalGuest.GetTableData<GMSFinalGuestDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestById(int GuestId)
    {
        try
        {
            string query = "Select * from MembersDetails where Id=@Id";
            var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(query, parameter);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestByIdWithChild(int GuestId)
    {
        try
        {
            string query = $"Select rt.RType RoomTypeName ,RoomType,gm.Gender as GenderName,md.*,c.City,s.State "
                          + " ,(Select top 1 tc.Country from tblCountries tc where tc.SrNo=md.Country)Country"
                          + " from MembersDetails md"
                          + " Left Join GenderMaster gm on md.Gender=gm.Id"
                          + " Left Join tblCity c on md.City=c.ID"
                          + " Left Join tblstate s on md.state=s.ID Left join RoomType rt on  md.RoomType = rt.ID "
                          + " where md.Id=@Id";
            var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.MembersDetails.GetEntityData<MemberDetailsWithChild>(query, parameter);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestsEventForCalender(int GuestId)
    {
        try
        {
            string query1 = $"Select * from GuestSchedule where guestid=@GuestId";
            string query = @"Select gs.*,tm.TaskName,wm.WorkerName,rm.ResourceName from GuestSchedule gs
                                left Join TaskMaster tm on gs.TaskId=tm.Id
                                left join EHRMS.dbo.WorkerMaster wm on gs.EmployeeId=wm.WorkerID
                                left join ResourceMaster rm on gs.ResourceId=rm.Id

                                where guestid=@GuestId";
            var parameter = new { @GuestId = GuestId };
            var res = await _unitOfWork.GuestSchedule.GetTableData<GuestScheduleWithChild>(query, parameter);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsEventForCalender)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestsEventForCalenderById(int Id)
    {
        try
        {
            string query = $"Select * from GuestSchedule where Id=@Id";
            var parameter = new { @Id = Id };
            var res = await _unitOfWork.GuestSchedule.GetEntityData<GuestScheduleDTO>(query, parameter);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsEventForCalender)}");
            throw;
        }
    }
    public async Task<IActionResult> CreateGuestScheduleByCalendar(GuestScheduleDTO dto)
    {
        try
        {
            if (dto != null)
            {
                if (dto.Id > 0)
                {
                    string query = $"Select * from GuestSchedule where Id=@Id";
                    var parameter = new { @Id = dto.Id };
                    GuestSchedule gs = await _unitOfWork.GuestSchedule.GetEntityData<GuestSchedule>(query, parameter);
                    gs.StartDateTime = dto.StartDateTime;
                    gs.EndDateTime = dto.EndDateTime;
                    gs.Duration = dto.Duration;
                    gs.TaskId = dto.TaskId;
                    gs.EmployeeId = dto.EmployeeId;
                    gs.ResourceId = dto.ResourceId;
                    var updated = await _unitOfWork.GuestSchedule.UpdateAsync(gs);
                    if (updated)
                    {
                        return Ok(dto);
                    }
                    else
                    {
                        return BadRequest("Unable to Update right now");
                    }
                }
                else
                {
                    dto.Id = await _unitOfWork.GuestSchedule.AddAsync(_mapper.Map<GuestSchedule>(dto));
                    if (dto.Id == 0)
                    {
                        return BadRequest("Unable to Add right now");
                    }
                    else
                    {
                        return Ok(dto);
                    }
                }
            }
            else
            {
                return BadRequest("Unable to update it");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsEventForCalender)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAvailableRoomsSharedStatusForGuest(int GuestId)
    {
        try
        {
            string query = "GetAvailableRoomsSharedStatusForGuest";
            var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.MembersDetails.ExecuteStoredProcedureAsync<AvailableRoomsSharedStatus>(query, parameter);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAvailableRoomsForGuestAllocation_New(int GuestId, int SharedStatus)
    {
        try
        {
            string query = "GetAvailableRoomsForGuestAllocation_New";
            var parameter = new { @Id = GuestId, @SharedStatus = SharedStatus };
            var res = await _unitOfWork.MembersDetails.ExecuteStoredProcedureAsync<AvailableRoomsForGuestAllocation>(query, parameter);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestByGroupId(string GroupId)
    {
        try
        {
            string query = "Select * from MembersDetails where GroupId=@GroupId";
            var parameter = new { @GroupId = GroupId };
            var res = await _unitOfWork.MembersDetails.GetTableData<MembersDetailsDTO>(query, parameter);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestByGroupIdAndPax(string GroupId, int PaxNo)
    {
        try
        {
            string existQuery = "Select * from MembersDetails where GroupId=@GroupId and PaxSno=@PaxSno";
            var existParameters = new { GroupId = GroupId, PaxSno = PaxNo };

            var isExists = await _unitOfWork.MembersDetails.IsExists(existQuery, existParameters);
            if (isExists)
            {
                string query = existQuery;
                var parameter = new { GroupId = GroupId, PaxSno = PaxNo };
                var res = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(query, parameter);
                return Ok(res);
            }
            else
            {
                string query = $"Select PaxCompleted,Id,ServiceId,Address1,Pincode,City,State,Country,Nationality,IsMonthlynewsletter,AboutUs,RelativeName,Relations,RelativeNumber,ReferContact,Remarks\r\n,Staydays,RoomType,CatID,IsCRM,Nights,GuarenteeCode,PaymentStatus,HoldTillDate,PaymentDate,LeadSource,ChannelCode,AdditionalNights,DateOfArrival,DateOfDepartment,PAX,NoOfRooms,PickUpDrop,PickUpType,CarType,FlightArrivalDateAndDateTime,FlightDepartureDateAndDateTime,PickUpLoaction,Age,CustomerName,GroupId,{PaxNo} as PaxSno from MembersDetails where GroupId=@GroupId and PaxSno=1";
                var parameter = new { @GroupId = GroupId };
                var res = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(query, parameter);
                return Ok(res);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GuestsListStatusWiseAndPageWise1(GuestsGridViewParameters inputDTO, string Action)
    {
        try
        {
            string pageDetails = "";
            string where = $"";
            string statusFilter = $"";
            string query = "";
            string selectData = "";
            string orderBy = "";
            if (inputDTO != null)
            {
                if (String.IsNullOrEmpty(inputDTO.SearchKeyword))
                {
                    inputDTO.SearchKeyword = "";
                }
                string searchFilter = "and";

                searchFilter += $" ( ";
                searchFilter += $" (CustomerName like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" or (RefNo like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" ) ";
                if (Action == "Data")
                {
                    selectData = $" Convert(date,DOA) DateOfArrival,convert(date,DOD) DateOfDeparture,gm.Gender GenderName,(case when (FG.DOB is null or ltrim(rtrim(FG.DOB))='') then 0 else DATEDIFF(year,convert(date,FG.DOB),getdate()) end )Age,FG.*   ";
                    orderBy += $" order by Convert(date,DOA) ";
                    if (inputDTO != null && inputDTO.PageNumber != null && inputDTO.PageSize != null)
                    {
                        pageDetails = $"OFFSET {(inputDTO.PageNumber - 1) * inputDTO.PageSize} ROWS FETCH NEXT {inputDTO.PageSize} ROWS ONLY";
                    }
                }
                else
                {
                    selectData = $"count(1)";
                }

                query = $"Select {selectData} FROM GMSFinalGuest FG left Join GenderMaster gm on fg.Gender=gm.Id ";

                if (inputDTO?.GuestsListType == "Current")
                {
                    statusFilter += $" and convert(date,getdate()) BETWEEN Convert(date,DOA) AND Convert(date,DOD) ";
                }
                else if (inputDTO?.GuestsListType == "All")
                {
                    statusFilter += $"  ";
                }
                else if (inputDTO?.GuestsListType == "Upcoming")
                {
                    statusFilter += $" and convert(date,getdate()) < Convert(date,DOA) ";
                }
                else if (inputDTO?.GuestsListType == "Previous")
                {
                    statusFilter += $" and convert(date,getdate()) > Convert(date,DOA) ";
                }

                string whereClause = " 1=1 ";
                whereClause += statusFilter + searchFilter;
                if (!String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where " + whereClause;
                }

                string finalQuery = query + whereClause + orderBy + pageDetails;



                if (Action == "Data")
                {
                    List<GMSFinalGuestsWithChild> dto = await _unitOfWork.GMSFinalGuest.GetTableData<GMSFinalGuestsWithChild>(finalQuery);
                    return Ok(dto);
                }
                else
                {
                    List<int> dto = await _unitOfWork.GMSFinalGuest.GetTableData<int>(finalQuery);
                    return Ok(dto);
                }


            }
            else
            {
                return BadRequest("");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GuestsListStatusWiseAndPageWise)}");
            throw;
        }
    }

    public async Task<IActionResult> GuestsListStatusWiseAndPageWise(GuestsGridViewParameters inputDTO, string Action)
    {
        try
        {
            string pageDetails = "";
            string where = $"";
            string statusFilter = $"";
            string query = "";
            string selectData = "";
            string orderBy = "";
            if (inputDTO != null)
            {
                if (String.IsNullOrEmpty(inputDTO.SearchKeyword))
                {
                    inputDTO.SearchKeyword = "";
                }
                string searchFilter = "and";

                searchFilter += $" ( ";
                searchFilter += $" (CustomerName like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" or (UniqueNo like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" or (MobileNo like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" ) ";
                if (Action == "Data")
                {
                    //selectData = $" (select count(*) from [dbo].[GuestsChkList] where GuestID=md.Id) InHouse,(Case when ra.RNumber is null then 1 when ra.CheckInDate is null then 2 when CheckOutDate is null then 3 else 4 end) CheckInStatus,md.PaxCompleted,md.GroupId,md.DOB, md.Id\r\n,UniqueNo\r\n,CustomerName\r\n,MobileNo\r\n,s.Service\r\n,mc.Category\r\n,md.Photo\r\n,IsApproved\r\n,md.CreationDate\r\n,md.Status\r\n,g.Gender GenderName\r\n,(select count(1) from [dbo].[GuestsChkList] gcl where gcl.GuestID=md.Id) IsChecked\r\n,md.Age\r\n,md.Nationality ,DateOfArrival ,DateOfDepartment,rt.RType\r\n,(SELECT STUFF((SELECT ', ' + convert(nvarchar(20),'Room No '+ra.RNumber) \r\n               FROM RoomAllocation ra \r\n               WHERE ra.GuestID = md.Id \r\n                 AND ra.FD = md.DateOfArrival \r\n                 AND ra.TD = md.DateOfDepartment \r\n                 AND ra.IsActive = 1 \r\n               FOR XML PATH('')), 1, 2, '')) AS RoomNumber   ";
                    selectData = $@" (select count(*) from [dbo].[GuestsChkList] where GuestID=md.Id) InHouse
                                    ,(Case when ra.RNumber is null then 1 when ra.CheckInDate is null then 2 when CheckOutDate is null then 3 else 4 end) CheckInStatus
                                    ,md.PaxCompleted
                                    ,md.GroupId
                                    ,md.DOB
                                    ,md.Id
                                    ,UniqueNo
                                    ,CustomerName
                                    ,MobileNo
                                    ,s.Service
                                    ,mc.Category
                                    ,md.Photo
                                    ,IsApproved
                                    ,md.CreationDate
                                    ,md.Status
                                    ,g.Gender GenderName
                                    ,(select count(1) from [dbo].[GuestsChkList] gcl where gcl.GuestID=md.Id) IsChecked
                                    ,md.Age
                                    ,md.Nationality 
                                    ,DateOfArrival 
                                    ,DateOfDepartment
                                    ,rt.RType
                                    ,(SELECT STUFF((SELECT ', ' + convert(nvarchar(20),'Room No '+ra.RNumber) 
                                        FROM RoomAllocation ra 
                                        WHERE ra.GuestID = md.Id 
                                        --AND ra.FD = md.DateOfArrival 
                                        --AND ra.TD = md.DateOfDepartment 
                                        AND ra.IsActive = 1 
                                        FOR XML PATH('')), 1, 2, '')) AS RoomNumber   ";
                    orderBy += $" order by md.Id ";
                    if (inputDTO != null && inputDTO.PageNumber != null && inputDTO.PageSize != null)
                    {
                        pageDetails = $"OFFSET {(inputDTO.PageNumber - 1) * inputDTO.PageSize} ROWS FETCH NEXT {inputDTO.PageSize} ROWS ONLY";
                    }
                }
                else
                {
                    selectData = $"count(1)";
                }

                query = $"Select {selectData} FROM MembersDetails md\r\nleft Join RoomType rt on md.RoomType=rt.ID\r\nleft Join GenderMaster g on md.Gender=g.Id\r\nleft join MstCategory mc on mc.ID=md.ServiceId\r\nleft join Services s on s.ID=md.CatID Left Join RoomAllocation ra on md.Id=ra.GuestID";

                if (inputDTO?.GuestsListType == "Current")
                {
                    statusFilter += $" and convert(date,getdate()) BETWEEN Convert(date,DateOfArrival) AND Convert(date,DateOfDepartment) ";
                }
                else if (inputDTO?.GuestsListType == "All")
                {
                    statusFilter += $"  ";
                }
                else if (inputDTO?.GuestsListType == "Upcoming")
                {
                    statusFilter += $" and convert(date,getdate()) < Convert(date,DateOfArrival) ";
                }
                else if (inputDTO?.GuestsListType == "Previous")
                {
                    statusFilter += $" and convert(date,getdate()) > Convert(date,DateOfArrival) ";
                }

                string whereClause = " md.Status=1 ";
                whereClause += statusFilter + searchFilter;
                if (!String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where " + whereClause;
                }

                string finalQuery = query + whereClause + orderBy + pageDetails;



                if (Action == "Data")
                {
                    List<MemberDetailsWithChild> dto = await _unitOfWork.GMSFinalGuest.GetTableData<MemberDetailsWithChild>(finalQuery);
                    return Ok(dto);
                }
                else
                {
                    List<int> dto = await _unitOfWork.GMSFinalGuest.GetTableData<int>(finalQuery);
                    return Ok(dto);
                }


            }
            else
            {
                return BadRequest("");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GuestsListStatusWiseAndPageWise)}");
            throw;
        }
    }
    public async Task<IActionResult> FetchDepartmentDate(MembersDetailsDTO inputDTO)
    {
        try
        {
            var parameters = new { Package = inputDTO.CatId };
            string query = "SELECT * FROM dbo.MstCategory mc WHERE id=@Package AND mc.Status=1 ";
            var res = await _unitOfWork.MstCategory.GetEntityData<MstCategoryDTO>(query, parameters);
            if (res != null)
            {
                int? totalNights = res.NoOfNights + inputDTO.AdditionalNights;
                double dTotalNights = Convert.ToDouble(totalNights);
                DateTime? departDate = inputDTO.DateOfArrival?.AddDays(dTotalNights);
                return Ok(departDate);
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<string> GenerateRandomNumberAndValidate()
    {
        try
        {
            string GroupId = "";
            bool isExits = false;
            do
            {
                GroupId = "GRP00" + CommonHelper.GenerateNaadRandomNo();//When Id=0
                isExits = await _unitOfWork.MembersDetails.IsExists($"Select GroupId from MembersDetails where GroupId={GroupId}", null);
            }
            while (isExits);
            return GroupId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<int> GetRoomTypeByRoomNumber(string RoomNumber)
    {
        try
        {
            string sQuery = "Select * from Rooms where RNumber=@RNumber";
            var sParam = new { @RNumber = RoomNumber };
            var rooms = await _unitOfWork.GenOperations.GetEntityData<RoomsDTO>(sQuery, sParam);
            if (rooms == null)
            {
                return 0;
            }
            else
            {
                return rooms.RtypeId ?? 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<int> GetPaxAndUpdate(string GroupId)
    {
        try
        {
            string sQuery = "Select GroupId,PAX,PAXSno,* from MembersDetails where GroupId=@GroupId order by Id desc";
            var sParam = new { @GroupId = GroupId };
            var guests = await _unitOfWork.GenOperations.GetTableData<MembersDetailsDTO>(sQuery, sParam);
            if (guests == null)
            {
                return 0;
            }
            else if (guests.Count > 2)
            {
                return -2;
            }
            else
            {
                string uQuery = "Update MembersDetails set PAX=@PAX where GroupId=@GroupId";
                var uParam = new { @GroupId = GroupId, @PAX = guests.Count + 1 };
                await _unitOfWork.GenOperations.RunSQLCommand(uQuery, uParam);
                return guests.Count + 1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> AddGuests(MembersDetailsDTO inputDTO)
    {
        try
        {
            string existsQuery = "select * from MembersDetails where  MobileNo=@MobileNo and [status]=1";
            var existsParameters = new { MobileNo = inputDTO.MobileNo };
            var exists = await _unitOfWork.MembersDetails.IsExists(existsQuery, existsParameters);
            if (!exists)
            {
                inputDTO.Id = await _unitOfWork.MembersDetails.AddAsync(_mapper.Map<MembersDetails>(inputDTO));
                if (inputDTO.Id > 0)
                {
                    return Ok(inputDTO);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest("Mobile Number Already Exists");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> UpdateGuests(MembersDetailsDTO inputDTO)
    {
        try
        {




            string existsQuery = "select * from MembersDetails where  MobileNo=@MobileNo and [status]=1 and Id!=@Id";
            var existsParameters = new { MobileNo = inputDTO.MobileNo, @Id = inputDTO.Id };
            var exists = await _unitOfWork.MembersDetails.IsExists(existsQuery, existsParameters);
            if (!exists)
            {
                string query = "Select * from MembersDetails where Id=@Id";
                var parameter = new { @Id = inputDTO.Id };
                var memberDetails = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(query, parameter);

                if (memberDetails != null)
                {
                    inputDTO.IsCrm = memberDetails.IsCrm;
                    inputDTO.Status = memberDetails.Status;
                    inputDTO.CreationDate = memberDetails.CreationDate;
                    inputDTO.AprovedDate = memberDetails.AprovedDate;
                    inputDTO.IsApproved = memberDetails.IsApproved;
                    inputDTO.ApprovedBy = memberDetails.ApprovedBy;
                    inputDTO.UniqueNo = memberDetails.UniqueNo;
                    inputDTO.Photo = memberDetails.Photo;

                    var res = await _unitOfWork.MembersDetails.UpdateAsync(_mapper.Map<MembersDetails>(inputDTO));
                    if (res)
                    {
                        return Ok(inputDTO.UniqueNo);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                return BadRequest();
            }
            else
            {
                return BadRequest("Mobile Number Already Exists");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> UpdatePAxCompleted(MembersDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO.GroupId != null)
            {
                var res = await _unitOfWork.MembersDetails.GetTableData<MembersDetails>($"Select * from MembersDetails where GroupId='{inputDTO.GroupId}'");
                if (res != null)
                {
                    foreach (var item in res)
                    {
                        item.PaxCompleted = true;
                        await _unitOfWork.MembersDetails.UpdateAsync(item);
                    }
                }
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }


    public async Task<IActionResult> ManageGuests(MembersDetailsDTO inputDTO)
    {
        try
        {
            string existsByGroupIdAndPaxQuery = "Select * from MembersDetails where GroupId=@GroupId and PaxSno=@PaxSno";
            var existsByGroupIdAndPaxParameters = new { @GroupId = inputDTO.GroupId, @PaxSno = inputDTO.PAXSno };

            var existsByGroupIdAndPax = await _unitOfWork.MembersDetails.IsExists(existsByGroupIdAndPaxQuery, existsByGroupIdAndPaxParameters);
            if (existsByGroupIdAndPax)
            {
                var memberDetailsDTO = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(existsByGroupIdAndPaxQuery, existsByGroupIdAndPaxParameters);
                inputDTO.Id = memberDetailsDTO.Id;

                string existsQuery = "select * from MembersDetails where  MobileNo=@MobileNo and [status]=1 and Id!=@Id";
                var existsParameters = new { MobileNo = inputDTO.MobileNo, @Id = inputDTO.Id };
                var exists = await _unitOfWork.MembersDetails.IsExists(existsQuery, existsParameters);
                if (!exists)
                {
                    string query = "Select * from MembersDetails where Id=@Id";
                    var parameter = new { @Id = inputDTO.Id };
                    var memberDetails = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(query, parameter);

                    if (memberDetails != null)
                    {
                        inputDTO.IsCrm = memberDetails.IsCrm;
                        inputDTO.Status = memberDetails.Status;
                        inputDTO.CreationDate = memberDetails.CreationDate;
                        inputDTO.AprovedDate = memberDetails.AprovedDate;
                        inputDTO.IsApproved = memberDetails.IsApproved;
                        inputDTO.ApprovedBy = memberDetails.ApprovedBy;
                        inputDTO.UniqueNo = memberDetails.UniqueNo;
                        inputDTO.Photo = memberDetails.Photo;

                        var res = await _unitOfWork.MembersDetails.UpdateAsync(_mapper.Map<MembersDetails>(inputDTO));
                        if (res)
                        {
                            return Ok(inputDTO);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    return BadRequest();
                }
                else
                {
                    return BadRequest("Mobile Number Already Exists");
                }

            }
            else
            {
                string existsQuery = "select * from MembersDetails where  MobileNo=@MobileNo and [status]=1";
                var existsParameters = new { MobileNo = inputDTO.MobileNo };
                var exists = await _unitOfWork.MembersDetails.IsExists(existsQuery, existsParameters);
                if (!exists)
                {
                    inputDTO.Id = await _unitOfWork.MembersDetails.AddAsync(_mapper.Map<MembersDetails>(inputDTO));
                    if (inputDTO.Id > 0)
                    {
                        return Ok(inputDTO);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest("Mobile Number Already Exists");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> AllocateRoom(RoomAllocationDTO inputDTO)
    {
        try
        {
            string existsQuery = "Select * from RoomAllocation where GuestId=@GuestId";
            var existsParameters = new { GuestId = inputDTO.GuestId };

            var existsRes = await _unitOfWork.RoomAllocation.IsExists(existsQuery, existsParameters);
            if (existsRes)
            {
                var roomAllocationExisting = await _unitOfWork.RoomAllocation.GetEntityData<RoomAllocation>(existsQuery, existsParameters);
                if (roomAllocationExisting != null)
                {
                    roomAllocationExisting.Rnumber = inputDTO.Rnumber;
                    roomAllocationExisting.Shared = inputDTO.Shared;
                    roomAllocationExisting.ModifiedDate = System.DateTime.Now;
                    var updated = await _unitOfWork.RoomAllocation.UpdateAsync(roomAllocationExisting);
                    if (updated)
                    {
                        return Ok("Room Updated Successfully");
                    }
                }
                return BadRequest("Room already allocated. Unable to change room right now");
            }
            MembersDetails membersDetails = await _unitOfWork.MembersDetails.FindByIdAsync(inputDTO.GuestId ?? default(int));
            inputDTO.Rtype = membersDetails.RoomType;
            inputDTO.Fd = membersDetails.DateOfArrival?.ToString("yyyy-MM-dd HH:mm");
            inputDTO.Td = membersDetails.DateOfDepartment?.ToString("yyyy-MM-dd  HH:mm");
            inputDTO.AsigndDate = DateTime.Now;
            inputDTO.IsActive = 1;

            inputDTO.Id = await _unitOfWork.RoomAllocation.AddAsync(_mapper.Map<RoomAllocation>(inputDTO));
            if (inputDTO.Id > 0) { return Ok(inputDTO); }
            else { return BadRequest("Some error has occurred while assigning room"); }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GuestCheckIn(MedicalSoultion_GuestCheckList inputDTO)
    {
        try
        {
            string spName = "CheckInGuest";
            var spParameters = new { @opt = inputDTO.opt, @checkList = inputDTO.checklist, @GuestId = inputDTO.ID };
            var res = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync<Object>(spName, spParameters);

            spName = "PopulateGuestSchedule_Latest";
            var spSchParam = new { @GuestId = inputDTO.ID };
            var res1 = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync<Object>(spName, spParameters);

            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GuestCheckInNew(MedicalSoultion_GuestCheckList inputDTO)
    {
        try
        {
            string eQuery = "Select * from roomallocation where GuestID=@GuestID";
            var eParam = new { @GuestID = inputDTO.ID };

            var eCheckInExists = await _unitOfWork.GenOperations.IsExists(eQuery, eParam);
            if (eCheckInExists)
            {
                var guestsCheckinDetails = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>(eQuery, eParam);
                if (guestsCheckinDetails != null)
                {
                    if (guestsCheckinDetails.CheckInDate != null)
                    {
                        return BadRequest("Guest already checked in");
                    }
                    else
                    {
                        guestsCheckinDetails.CheckInDate = DateTime.Now;
                        var checkInDateUpdated = await _unitOfWork.RoomAllocation.UpdateAsync(guestsCheckinDetails);
                        if (checkInDateUpdated)
                        {
                            string spName = "CheckInGuest";
                            var spParameters = new { @opt = inputDTO.opt, @checkList = inputDTO.checklist, @GuestId = inputDTO.ID };
                            var res = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync<Object>(spName, spParameters);

                            spName = "PopulateGuestSchedule_Latest";
                            var spSchParam = new { @GuestId = inputDTO.ID };
                            var res1 = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync<Object>(spName, spSchParam);

                            return Ok("Checked In Successfully");
                        }
                        else
                        {
                            return BadRequest("Unable to check in right now");
                        }
                    }
                }
                else
                {
                    return BadRequest("Please assign a room to this guest first");
                }
            }
            else
            {
                return BadRequest("Please assign a room to this guest first");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> GuestCheckOut(MedicalSoultion_GuestCheckList inputDTO)
    {
        try
        {
            string eQuery = "Select * from roomallocation where GuestID=@GuestID";
            var eParam = new { @GuestID = inputDTO.ID };

            var eCheckInExists = await _unitOfWork.GenOperations.IsExists(eQuery, eParam);
            if (eCheckInExists)
            {
                var guestsCheckinDetails = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>(eQuery, eParam);
                if (guestsCheckinDetails != null)
                {
                    if (guestsCheckinDetails.CheckInDate == null)
                    {
                        return BadRequest("Guest is not checked in");
                    }
                    else if (guestsCheckinDetails.CheckOutDate == null)
                    {
                        guestsCheckinDetails.CheckOutDate = DateTime.Now;
                        guestsCheckinDetails.Td = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
                        var checkOutDateUpdated = await _unitOfWork.RoomAllocation.UpdateAsync(guestsCheckinDetails);
                        if (checkOutDateUpdated)
                        {
                            string spName = "CheckInGuest";
                            var spParameters = new { @opt = 1, @checkList = inputDTO.checklist, @GuestId = inputDTO.ID };
                            var res = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync<Object>(spName, spParameters);

                            //spName = "PopulateGuestSchedule_Latest";
                            //var spSchParam = new { @GuestId = inputDTO.ID };
                            //var res1 = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync<Object>(spName, spSchParam);

                            return Ok("Checked Out Successfully");
                        }
                        else
                        {
                            return BadRequest("Unable to check out right now");
                        }
                    }
                    else
                    {
                        return BadRequest("Guest is already checked out");
                    }
                }
                else
                {
                    return BadRequest("Please assign a room to this guest first");
                }
            }
            else
            {
                return BadRequest("Please assign a room to this guest first");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GetTasks()
    {
        try
        {
            string sQuery = "Select * from TaskMaster where IsDeleted=0 and IsActive=1";
            var res = await _unitOfWork.TaskMaster.GetTableData<TaskMasterDTO>(sQuery);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GetEmployeeByTaskId(int TaskId)
    {
        try
        {
            string sQuery = "Select WorkerID EmployeeId,WorkerName EmployeeName,EMPID EmployeeCode from EHRMS.dbo.WorkerMaster where roleid=(Select Department from TaskMaster where id=@TaskId) and isactive='Y'";
            var sParam = new { @TaskId = TaskId };
            var res = await _unitOfWork.TaskMaster.GetTableData<EmployeeMasterKeyValue>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GetResourcesByTaskId(int TaskId)
    {
        try
        {
            string sQuery = "Select * from ResourceMaster where DepartmentId=(Select Department from TaskMaster where id=@TaskId) and IsActive=1 and IsDeleted=0";
            var sParam = new { @TaskId = TaskId };
            var res = await _unitOfWork.ResourceMaster.GetTableData<ResourceMasterDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestsDetailsInRooms(string RoomNumber, DateTime DateValue)
    {
        try
        {
            string sQuery = @"Select 
                                md.Id,md.CustomerName,md.MobileNo,md.UniqueNo,md.GroupId,md.DateOfArrival,md.DateOfDepartment
                                from RoomAllocation ra
                                Left Join MembersDetails md on ra.GuestID=md.Id
                                where RNumber=@RoomNumber
                                and IsActive=1 and cast(@DateValue as date) between cast(ra.FD as date) and cast(ra.TD as date)";
            var sParam = new { @RoomNumber = RoomNumber, @DateValue };
            var res = await _unitOfWork.ResourceMaster.GetTableData<MembersDetailsDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestCheckList(string CheckListType)
    {
        try
        {
            string sQuery = "Select * from TblCheckLists where IsActive=1 and ChecklistType=@ChecklistType";
            var sParam = new { @ChecklistType = CheckListType };
            var res = await _unitOfWork.GenOperations.GetTableData<TblCheckListsDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestDetailsByPhoneNumber(MembersDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.PhNo))
            {
                string sQuery = @"Select CustomerName,gm.Gender GenderName , md.* from MembersDetails md
                                Left Join GenderMaster gm on md.Gender=gm.Id
                                where MobileNo like '%'+@Phno+'%'";
                var sParam = new { @Phno = inputDTO.PhNo };
                var res = await _unitOfWork.MembersDetails.GetTableData<MemberDetailsWithChild>(sQuery, sParam);
                return Ok(res);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

}
