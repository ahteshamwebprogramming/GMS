using AutoMapper;
using Dapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.Accounting;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Guests;
using GMS.Infrastructure.ViewModels.Masters;
using GMS.Infrastructure.ViewModels.Rooms;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
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
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public GuestsAPIController(IUnitOfWork unitOfWork, ILogger<GuestsAPIController> logger, IMapper mapper, IWebHostEnvironment hostingEnv)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _hostingEnv = hostingEnv;
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
    //public async Task<IActionResult> GetGuestByIdWithOutStayInformation(int GuestId)
    //{
    //    try
    //    {
    //        string query = "Select * from MembersDetails where Id=@Id";
    //        var parameter = new { @Id = GuestId };
    //        var res = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(query, parameter);
    //        return Ok(res);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
    //        throw;
    //    }
    //}
    public async Task<IActionResult> GetGuestStayInformationByGroupId(string GroupId)
    {
        try
        {
            string query = "Select * from MembersDetails where GroupId=@GroupId and PAXSno=1";
            var parameter = new { @GroupId = GroupId };
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

            string query1 = @"Select 
                            md.Id
                            ,rt.RType RoomTypeName 
                            ,RoomType
                            ,gm.Gender as GenderName
                            ,[FName]
                            ,[MName]
                            ,[LName]
                            ,CustomerName
                            ,[PhNo]
                            ,countries.iso3 [CountryCode]
                            ,[MobileNo]
                            ,[EmergencyNo]
                            ,[DOB]
                            ,(Case when [MarStatus]='S' then 'Single' when [MarStatus]='M' then 'Married' when [MarStatus]='Se' then 'Separated' when [MarStatus]='D' then 'Divorce' when [MarStatus]='O' then 'Others'  else '' end)   [MarStatus]
                                       ,md.[Gender]
                                       ,[PhysicianName]
                                       ,[PhysicianNumber]
                                       ,[ReferredBy]
                                       ,[ServiceId]
                                       ,[LegalName]
                                       ,[Address1]
                                       ,[Address2]
                                       ,[Pincode]
                                       ,[Country]
                                       ,[Email]
                                       ,[IDProof]
                                       ,[IsMonthlynewsletter]
                                       ,[AboutUs]
                                       ,[RelativeName]
                                       ,[Relations]
                                       ,[RelativeNumber]
                                       ,[ReferContact]
                                       ,[UniqueNo]
                                       ,md.[Status]
                                       ,[IsApproved]
                                       ,md.[Remarks]
                                       ,[UserId]
                                       ,[ApprovedBy]
                                       ,[CreationDate]
                                       ,[AprovedDate]
                                       ,[Photo]
                                       ,[BloodGroup]
                                       ,[Occupation]
                                       ,[PassportNo]
									   ,PassportIssueDate
									   ,PassportExpiryDate
                                       ,[VisaDetails]
									   ,PhotoShared
									   ,VisaIssueDate
									   ,VisaExpiryDate
                                       ,IDProof
                                       ,IDProofIssueDate
                                       ,IdProofExpiryDate
                                       ,[PolicyHolder]
                                       ,[InsuranceCompany]
                                       ,[PolicyNo]
                                       ,[CoveredIndia]
                                       ,[Staydays]
                                       ,[RoomType]
                                       ,[HealthIssues]
                                       ,[CatID]
                                       ,[IsCRM]
                                       ,[Nights]
                                       ,countries.nationality [Nationality]
                                       ,NationalityId
                                       ,[GuarenteeCode]
                                       ,[PaymentStatus]
                                       ,[HoldTillDate]
                                       ,[PaymentDate]
                                       ,[LeadSource]
                                       ,[ChannelCode]
                                       ,[AdditionalNights]
                                       ,(Case when ra.CheckInDate is not null then ra.CheckInDate else md.[DateOfArrival] end)[DateOfArrival]
                                       ,(Case when ra.CheckOutDate is not null then ra.CheckOutDate else md.[DateOfDepartment] end)[DateOfDepartment]
                                       ,DateOfArrival [ActDateOfArrival]
                                       ,DateOfDepartment [ActDateOfDepartment]
                                       ,[PAX]
                                       ,[NoOfRooms]
                                       ,[PickUpDrop]
                                       ,[PickUpType]
                                       ,[CarType]
                                       ,[FlightArrivalDateAndDateTime]
                                       ,[FlightDepartureDateAndDateTime]
                                       ,[PickUpLoaction]
                                       ,[Age]
                                       ,[GroupId]
                                       ,[PAXSno]
                                       ,[PaxCompleted]
                                       ,[RegistrationNumber]
                                       ,[UHID]
                                       ,isnull((Select AVG(Price) from rates r where r.[Date] between md.DateOfArrival and md.DateOfDepartment and  roomtypeid=md.RoomType and PlanId=md.CatID),(SELECT TOP 1 r.Price FROM Rates r WHERE r.Date <= ISNULL(ra.CheckInDate, md.DateOfArrival) AND r.RoomTypeId = md.RoomType AND r.PlanId = md.CatID ORDER BY r.Date DESC ))RoomPrice
                            ,c.name City
                            ,s.name State  
                            ,countries.name  Country
                            ,ra.RNumber RoomNumber
                            ,ss.[Service]
                            --,isnull(md.ServiceId,0) + isnull(md.AdditionalNights,0) as NoOfNights
                            ,DATEDIFF(DAY,  isnull(ra.CheckInDate,md.DateOfArrival) ,  isnull(ra.CheckOutDate,md.DateOfDepartment) ) NoOfNights
                            ,ss.Price PackagePrice
                            from MembersDetails md 
                            Left Join GenderMaster gm on md.Gender=gm.Id 
                            Left Join cities c on md.CityId=c.ID 
                            Left Join states s on md.StateId=s.ID
                            Left Join countries countries on countries.id=md.CountryId
                            Left join RoomType rt on  md.RoomType = rt.ID  
                            Left Join RoomAllocation ra on ra.GuestID=md.Id
                            Left Join Services ss on md.CatID=ss.ID
                            where md.Id=@Id";
            var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.MembersDetails.GetEntityData<MemberDetailsWithChild>(query1, parameter);
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
            string query = @"Select gs.*,tm.TaskName,wm1.WorkerName EmployeeName1,wm2.WorkerName EmployeeName2,rm.ResourceName from GuestSchedule gs
                                left Join TaskMaster tm on gs.TaskId=tm.Id
                                left join EHRMS.dbo.WorkerMaster wm1 on gs.EmployeeId1=wm1.WorkerID
								left join EHRMS.dbo.WorkerMaster wm2 on gs.EmployeeId2=wm2.WorkerID
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
    public async Task<bool> AccountSettled(int GuestId)
    {
        try
        {
            string squery = @"Select * from Settlement s where GuestId in (
                                SELECT md.Id
                                --, md.CustomerName, md.GroupId, ra.Rnumber
                                FROM MembersDetails md
                                INNER JOIN RoomAllocation ra ON md.Id = ra.GuestID AND ra.IsActive = 1 and md.Status=1
                                WHERE md.GroupId = (
                                    SELECT md2.GroupId
                                    FROM MembersDetails md2
                                    WHERE md2.Id = @GuestId and md2.Status=1
                                )
                                AND ra.Rnumber = (
                                    SELECT ra2.Rnumber
                                    FROM RoomAllocation ra2
                                    WHERE ra2.GuestID = @GuestId AND ra2.IsActive = 1
                                )
                                ) and s.isactive=1;";
            var res = await _unitOfWork.GenOperations.GetEntityCount(squery, new { @GuestId = GuestId });
            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                    gs.EmployeeId1 = dto.EmployeeId1;
                    gs.EmployeeId2 = dto.EmployeeId2;
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
                    DateTime? guestEndStayDate = DateTime.Now.AddDays(1);
                    string memberDetailsQuery = "Select * from MembersDetails where Id=@Id";
                    var memberDetailsParam = new { @Id = dto.GuestId };
                    var memberDetails = await _unitOfWork.GenOperations.GetEntityData<MembersDetails>(memberDetailsQuery, memberDetailsParam);
                    if (memberDetails != null)
                    {
                        guestEndStayDate = memberDetails.DateOfDepartment;
                    }

                    int iteration = 1;//dto.SessionId ?? 1;
                    int iterationLimit = dto.SessionId ?? 1;
                    string query = @"Select * from GuestSchedule gs where gs.GuestId=@GuestId AND (
                                    -- Case 1: StartDateTime falls within an existing schedule
                                    (@StartDateTime BETWEEN gs.StartDateTime AND gs.EndDateTime)
    
                                    -- Case 2: EndDateTime falls within an existing schedule
                                    OR (@EndDateTime BETWEEN gs.StartDateTime AND gs.EndDateTime)
    
                                    -- Case 3: Existing schedule falls completely within the given range
                                    OR (gs.StartDateTime BETWEEN @StartDateTime AND @EndDateTime)
    
                                    -- Case 4: The given range fully covers an existing schedule
                                    OR (gs.EndDateTime BETWEEN @StartDateTime AND @EndDateTime)
                                ) order by EndDateTime desc;";
                    DateTime StartDateTime = dto.StartDateTime;
                    DateTime EndDateTime = dto.EndDateTime;

                    do
                    {
                        var param = new { @GuestId = dto.GuestId, @StartDateTime = StartDateTime, @EndDateTime = EndDateTime };
                        bool scheduleExists = await _unitOfWork.GenOperations.IsExists(query, param);
                        if (scheduleExists)
                        {
                            var res = await _unitOfWork.GenOperations.GetTableData<GuestSchedule>(query, param);
                            if (res != null && res.Count > 0 && res.FirstOrDefault() != null)
                            {
                                StartDateTime = res.FirstOrDefault().EndDateTime.AddMinutes(1);
                                EndDateTime = StartDateTime.Add(dto.Duration).AddMinutes(-1);

                            }
                        }
                        else
                        {
                            if (EndDateTime > guestEndStayDate?.AddMinutes(-60))
                            {
                                break;
                            }

                            GuestSchedule schedule = new GuestSchedule();
                            schedule.GuestId = dto.GuestId;
                            schedule.StartDateTime = StartDateTime;
                            schedule.EndDateTime = EndDateTime;
                            schedule.Duration = dto.Duration;
                            schedule.TaskId = dto.TaskId;
                            schedule.EmployeeId1 = dto.EmployeeId1;
                            schedule.EmployeeId2 = dto.EmployeeId2;
                            schedule.ResourceId = dto.ResourceId;
                            schedule.SessionId = iteration;


                            //dto.StartDateTime = StartDateTime;
                            //dto.EndDateTime = EndDateTime;
                            //dto.SessionId = iteration;
                            dto.Id = await _unitOfWork.GuestSchedule.AddAsync(schedule);
                            if (dto.Id == 0)
                            {
                                return BadRequest("Unable to Add right now");
                            }
                            StartDateTime = dto.StartDateTime.AddDays(iteration);
                            EndDateTime = dto.EndDateTime.AddDays(iteration);
                            iteration += 1;
                        }
                    }
                    while (iteration <= iterationLimit);


                    return Ok(dto);
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
            string query = "Select *,(Select top 1 ra.RNumber from RoomAllocation ra where GuestID=md.Id)RoomNumber from MembersDetails md where GroupId=@GroupId";
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
                string query = $"Select PaxCompleted,Id,ServiceId,Address1,Pincode,City,State,Country,Nationality,CityId,StateId,CountryId,NationalityId,IsMonthlynewsletter,AboutUs,RelativeName,Relations,RelativeNumber,ReferContact,Remarks\r\n,Staydays,RoomType,CatID,IsCRM,Nights,GuarenteeCode,PaymentStatus,HoldTillDate,PaymentDate,LeadSource,ChannelCode,AdditionalNights,DateOfArrival,DateOfDepartment,PAX,NoOfRooms,PickUpDrop,PickUpType,CarType,FlightArrivalDateAndDateTime,FlightDepartureDateAndDateTime,PickUpLoaction,Age,CustomerName,GroupId,{PaxNo} as PaxSno from MembersDetails where GroupId=@GroupId and PaxSno=1";
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

    public async Task<IActionResult> GetGuestAttachmentsByGroupIdAndPax(string GroupId, int PaxNo)
    {
        try
        {
            string sQuery = "Select * from MembersDetails where GroupId=@GroupId and PaxSno=@PaxSno";
            var sParameters = new { GroupId = GroupId, PaxSno = PaxNo };

            var guest = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(sQuery, sParameters);
            if (guest != null)
            {
                string sAQuery = "Select * from GuestDocumentAttachments where ReferenceId=@GuestId and IsActive=1";
                var sAParam = new { @GuestId = guest.Id };
                var res = await _unitOfWork.GuestDocumentAttachments.GetTableData<GuestDocumentAttachmentsDTO>(sAQuery, sAParam);
                return Ok(res);
            }
            return BadRequest("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetGuestAttachmentsByGuestId(int GuestId)
    {
        try
        {
            string sAQuery = "Select * from GuestDocumentAttachments where ReferenceId=@GuestId and IsActive=1";
            var sAParam = new { @GuestId = GuestId };
            var res = await _unitOfWork.GuestDocumentAttachments.GetTableData<GuestDocumentAttachmentsDTO>(sAQuery, sAParam);
            return Ok(res);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestAttachmentsByGuestId)}");
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

                if (inputDTO?.GuestsListType == "Cancelled")
                {
                    statusFilter += $" and isnull(ra.Cancelled,0)=1";
                }
                else
                {
                    statusFilter += $" and isnull(ra.Cancelled,0)=0";

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
                searchFilter += $" (dbo.NormalizeSpaces(CustomerName) like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" or (UHID like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" or (GroupId like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" or (rt.RType like '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" OR (FORMAT(isnull(isnull(ra.CheckOutDate,DateOfDepartment),ra.TD), 'dd-MMM-yyyy') LIKE '%{inputDTO.SearchKeyword}%')";
                searchFilter += $" or (Replace(MobileNo,' ','') like Replace('%{inputDTO.SearchKeyword}%',' ',''))";
                searchFilter += $" ) ";
                if (Action == "Data")
                {
                    //selectData = $" (select count(*) from [dbo].[GuestsChkList] where GuestID=md.Id) InHouse,(Case when ra.RNumber is null then 1 when ra.CheckInDate is null then 2 when CheckOutDate is null then 3 else 4 end) CheckInStatus,md.PaxCompleted,md.GroupId,md.DOB, md.Id\r\n,UniqueNo\r\n,CustomerName\r\n,MobileNo\r\n,s.Service\r\n,mc.Category\r\n,md.Photo\r\n,IsApproved\r\n,md.CreationDate\r\n,md.Status\r\n,g.Gender GenderName\r\n,(select count(1) from [dbo].[GuestsChkList] gcl where gcl.GuestID=md.Id) IsChecked\r\n,md.Age\r\n,md.Nationality ,DateOfArrival ,DateOfDepartment,rt.RType\r\n,(SELECT STUFF((SELECT ', ' + convert(nvarchar(20),'Room No '+ra.RNumber) \r\n               FROM RoomAllocation ra \r\n               WHERE ra.GuestID = md.Id \r\n                 AND ra.FD = md.DateOfArrival \r\n                 AND ra.TD = md.DateOfDepartment \r\n                 AND ra.IsActive = 1 \r\n               FOR XML PATH('')), 1, 2, '')) AS RoomNumber   ";
                    selectData = $@" (select count(*) from [dbo].[GuestsChkList] where GuestID=md.Id) InHouse
                                    ,(Case when ra.RNumber is null then 1 when ra.CheckInDate is null then 2 when CheckOutDate is null then 3 else 4 end) CheckInStatus
                                    ,(Select  top 1 tm.TaskName from GuestSchedule gs Left Join TaskMaster tm on gs.TaskId=tm.Id where GuestId=md.Id and EndDateTime >= getdate() order by EndDateTime asc) GuestTask
                                    ,(Select  top 1 gs.StartDateTime from GuestSchedule gs Left Join TaskMaster tm on gs.TaskId=tm.Id where GuestId=md.Id and EndDateTime >= getdate() order by EndDateTime asc) ActivityStartTime
									,(Select  top 1 gs.EndDateTime from GuestSchedule gs Left Join TaskMaster tm on gs.TaskId=tm.Id where GuestId=md.Id and EndDateTime >= getdate() order by EndDateTime asc) ActivityEndTime
                                    ,md.PaxCompleted
                                    ,md.GroupId
                                    ,md.DOB
                                    ,md.Id
                                    ,UniqueNo
                                    ,md.UHID
                                    ,CustomerName
                                    ,MobileNo
                                    ,s.Service
                                    ,(md.ServiceId + ' NIGHTS')Category
                                    ,md.Photo
                                    ,IsApproved
                                    ,md.CreationDate
                                    ,md.Status
                                    ,g.Gender GenderName
                                    ,(select count(1) from [dbo].[GuestsChkList] gcl where gcl.GuestID=md.Id) IsChecked
                                    ,md.Age
                                    ,md.Nationality 
                                    ,isnull(ra.Cancelled,0) Cancelled
                                    ,isnull(isnull(ra.CheckInDate,DateOfArrival),ra.FD)DateOfArrival 
                                    ,isnull(isnull(ra.CheckOutDate,DateOfDepartment),ra.TD)DateOfDepartment
                                    ,rt.RType
                                    ,rt.RType RoomTypeName
                                    ,(SELECT STUFF((SELECT ', ' + convert(nvarchar(20),'Room No '+ra.RNumber) 
                                        FROM RoomAllocation ra 
                                        WHERE ra.GuestID = md.Id 
                                        --AND ra.FD = md.DateOfArrival 
                                        --AND ra.TD = md.DateOfDepartment 
                                        AND ra.IsActive = 1 
                                        FOR XML PATH('')), 1, 2, '')) AS RoomNumber   
                                    ,(Select count(1) from Settlement s where s.GuestId=md.Id and IsActive=1) IsSettled";
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

                query = $"Select {selectData} FROM MembersDetails md\r\nleft Join RoomType rt on md.RoomType=rt.ID\r\nleft Join GenderMaster g on md.Gender=g.Id\r\nleft join MstCategory mc on mc.ID=md.ServiceId\r\nleft join Services s on s.ID=md.CatID Left Join RoomAllocation ra on md.Id=ra.GuestID and ra.IsActive=1";

                if (inputDTO?.GuestsListType == "Cancelled")
                {
                    statusFilter += $" and isnull(ra.Cancelled,0)=1";
                }
                else
                {
                    statusFilter += $" and isnull(ra.Cancelled,0)=0";

                    if (inputDTO?.GuestsListType == "Current")
                    {
                        statusFilter += $" and ( convert(datetime,getdate()) BETWEEN Convert(datetime,isnull(ra.CheckInDate,DateOfArrival)) AND Convert(datetime,isnull(ra.CheckOutDate,DateOfDepartment)) or (Convert(datetime,ra.CheckInDate) is not null and  Convert(datetime,ra.CheckOutDate) is null ) )";
                        //statusFilter += $" and convert(datetime,getdate()) BETWEEN Convert(datetime,isnull(ra.CheckInDate,DateOfArrival)) AND Convert(datetime,isnull(ra.CheckOutDate,DateOfDepartment)) ";
                    }
                    else if (inputDTO?.GuestsListType == "All")
                    {
                        statusFilter += $"  ";
                    }
                    else if (inputDTO?.GuestsListType == "Upcoming")
                    {
                        statusFilter += $" and convert(datetime,getdate()) < Convert(datetime,isnull(ra.CheckInDate,DateOfArrival)) ";
                    }
                    else if (inputDTO?.GuestsListType == "Previous")
                    {
                        statusFilter += $" and convert(datetime,getdate()) > Convert(datetime,isnull(ra.CheckInDate,DateOfArrival)) ";
                    }
                }
                string whereClause = " md.Status=1  ";
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

    public async Task<string> GetRegistrationNo(MembersDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO != null && inputDTO.MobileNo != null && String.IsNullOrEmpty(inputDTO.RegistrationNumber))
            {
                string sQuery = "Select * from MembersDetails where mobileno=@mobileno";
                inputDTO.MobileNo = Regex.Replace(inputDTO.MobileNo, @"\s+", "");
                var sParam = new { @mobileno = inputDTO.MobileNo.Trim() };
                var memberDetails = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(sQuery, sParam);
                if (memberDetails != null && !String.IsNullOrEmpty(memberDetails.RegistrationNumber))
                {
                    return memberDetails.RegistrationNumber;
                }
            }
            else if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.RegistrationNumber))
            {
                return inputDTO.RegistrationNumber;
            }

            string RegistrationNumber = await GenerateRegistrationNumberAndValidate();
            return RegistrationNumber;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<string> GenerateRegistrationNumberAndValidate()
    {
        try
        {
            string prefix = DateTime.Now.ToString("yyyyMM") + "NW";
            string query = $@"
            SELECT ISNULL(MAX(CAST(SUBSTRING(RegistrationNumber, LEN(@prefix) + 1, LEN(RegistrationNumber)) AS INT)), 99)
            FROM MembersDetails
            WHERE RegistrationNumber LIKE @prefix + '%'";

            // Get the current max number from DB
            int registrationNo2 = await _unitOfWork.MembersDetails.GetEntityData<int>(query, new { prefix });

            string registrationNo;
            bool exists;

            do
            {
                registrationNo2++;
                registrationNo = prefix + registrationNo2.ToString().PadLeft(7, '0');

                exists = await _unitOfWork.MembersDetails.IsExists(
                    "SELECT 1 FROM MembersDetails WHERE RegistrationNumber = @regNo",
                    new { regNo = registrationNo }
                );
            }
            while (exists);

            return registrationNo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating registration number in {nameof(GenerateRegistrationNumberAndValidate)}");
            throw;
        }
    }


    public async Task<string> GetUHIDFromMobileNo(string mobileno, string GuestName)
    {
        try
        {
            if (mobileno.Length < 10)
            {
                return "";
            }
            string sQuery = "SELECT UHID, GroupId FROM MembersDetails WHERE Replace(LTRIM(RTRIM(MobileNo)),' ','') = Replace(@MobileNo,' ','') AND dbo.NormalizeSpaces(CustomerName) = dbo.NormalizeSpaces(@GuestName) ORDER BY Id DESC;";
            var sParam = new { @MobileNo = mobileno, @GuestName = GuestName };
            var membersdetails = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(sQuery, sParam);
            return membersdetails?.UHID ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<string> GenerateUHIDAndValidate(string mobileno, string GuestName)
    {
        try
        {
            string uhidFromDB = await GetUHIDFromMobileNo(mobileno, GuestName);
            if (!String.IsNullOrEmpty(uhidFromDB))
            {
                return uhidFromDB;
            }

            string sQuery = "Select max(UHID) from MembersDetails";
            //var sParam = new { @mobileno = inputDTO.MobileNo };
            string uhid = await _unitOfWork.MembersDetails.GetEntityData<string>(sQuery);
            int intuhid = 0000099;
            if (uhid != null)
            {
                string struhid = uhid.Substring(6, 7);
                intuhid = int.TryParse(struhid, out intuhid) == true ? intuhid : 0000099;
            }
            bool isExits = false;
            do
            {
                intuhid = intuhid + 1;
                uhid = "1802NW" + intuhid.ToString().PadLeft(7, '0');
                isExits = await _unitOfWork.MembersDetails.IsExists($"Select UHID from MembersDetails where UHID='{uhid}'", null);
            }
            while (isExits);
            return uhid;
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
    public async Task<IActionResult> GetRoomsAvailableForGuestsNonShared(int GuestId)
    {
        try
        {
            string sQuery = @"DECLARE @StartDate DATETIME='',@EndDate DATETIME='',@RTypeID INT=0,@Status INT = 1;
                            Select @StartDate = (Case when md.DateOfArrival > getdate() then md.DateOfArrival else getdate() end),@EndDate=md.DateOfDepartment,@RTypeID=md.RoomType from MembersDetails md where Id=@GuestId;
                                                            WITH AvailableRooms AS (
                                                              SELECT r.RNumber
                                                              FROM Rooms r
                                                              WHERE r.RTypeID = @RTypeID
                                                                AND r.Status = @Status
                                                                AND NOT EXISTS (
                                                                  SELECT 1
                                                                  FROM RoomLock rl
                                                                  WHERE rl.Status = 1
                                                                    AND rl.[Type] IN ('Lock', 'Hold')
                                                                    AND (
                                                                          (rl.Rooms IS NOT NULL AND rl.Rooms = r.RNumber)
                                                                       OR (rl.Rooms IS NULL AND rl.RType = r.RTypeID)
                                                                        )
                                                                    AND (
                                                                          (rl.FD IS NULL OR CAST(rl.FD AS DATE) <= CAST(@EndDate AS DATE))
                                                                      AND (rl.ED IS NULL OR CAST(rl.ED AS DATE) >= CAST(@StartDate AS DATE))
                                                                        )
                                                                )
                                                                AND r.RNumber NOT IN (
                                                                  SELECT isnull(ra.RNumber,'')
                                                                  FROM RoomAllocation ra
                                                                  WHERE ra.IsActive = 1
                                                                    AND ISNULL(ra.Cancelled, 0) = 0
                                                                    --AND (
                                                                    --  -- Case 1: Both CheckInDate and CheckOutDate are NOT NULL
                                                                    --  (
                                                                    --    ra.CheckInDate IS NOT NULL
                                                                    --    AND ra.CheckOutDate IS NOT NULL
                                                                    --    AND (ra.CheckInDate <= @EndDate AND ra.CheckOutDate >= @StartDate)
                                                                    --  )
                                                                    --  OR
                                                                    --  -- Case 2: CheckInDate is NOT NULL, but CheckOutDate is NULL (indefinite occupancy)
                                                                    --  (
                                                                    --    ra.CheckInDate IS NOT NULL
                                                                    --    AND ra.CheckOutDate IS NULL
                                                                    --    AND (ra.CheckInDate <= @EndDate)
                                                                    --  )
                                                                    --  OR
                                                                    --  -- Case 3: Both CheckInDate and CheckOutDate are NULL (fall back to FD and TD)
                                                                    --  (
                                                                    --    ra.CheckInDate IS NULL
                                                                    --    AND ra.CheckOutDate IS NULL
                                                                    --    AND (ra.FD <= @EndDate AND ra.TD >= @StartDate)
                                                                    --  )
                                                                    --)
                                                                    AND (
                                                                        @StartDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)  
                                                                        OR 
                                                                        @EndDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)
                                                                        OR
                                                                        ISNULL(CheckInDate, FD) BETWEEN @StartDate AND @EndDate  
                                                                        OR
                                                                        ISNULL(CheckOutDate, TD) BETWEEN @StartDate AND @EndDate
                                                                    )
                                                                )

                                                            )
                                                            Select RNumber RoomNo from AvailableRooms";
            var sParameters = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.GetTableData<AvailableRoomsForGuestAllocation>(sQuery, sParameters);
            return Ok(res);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomsAvailableForGuestsShared(int GuestId)
    {
        try
        {
            string sQuery = @"DECLARE @StartDate DATETIME='',@EndDate DATETIME='',@RTypeID INT=0,@Status INT = 1;
Select @StartDate = (Case when md.DateOfArrival > getdate() then md.DateOfArrival else getdate() end),@EndDate=md.DateOfDepartment,@RTypeID=md.RoomType from MembersDetails md where Id=@GuestId;
                                                            WITH AvailableRooms AS (
                                                              SELECT r.RNumber
                                                              FROM Rooms r
                                                              WHERE r.RTypeID = @RTypeID
                                                                AND r.Status = @Status
                                                                AND NOT EXISTS (
                                                                  SELECT 1
                                                                  FROM RoomLock rl
                                                                  WHERE rl.Status = 1
                                                                    AND rl.[Type] IN ('Lock', 'Hold')
                                                                    AND (
                                                                          (rl.Rooms IS NOT NULL AND rl.Rooms = r.RNumber)
                                                                       OR (rl.Rooms IS NULL AND rl.RType = r.RTypeID)
                                                                        )
                                                                    AND (
                                                                          (rl.FD IS NULL OR CAST(rl.FD AS DATE) <= CAST(@EndDate AS DATE))
                                                                      AND (rl.ED IS NULL OR CAST(rl.ED AS DATE) >= CAST(@StartDate AS DATE))
                                                                        )
                                                                )
                                                                AND r.RNumber NOT IN (
                                                                  SELECT isnull(ra.RNumber,'')
                                                                  FROM RoomAllocation ra
                                                                  WHERE ra.IsActive = 1 and ISNULL(ra.Cancelled, 0) = 0 and ra.Shared=2
                                                                    --AND (
                                                                      -- Case 1: Both CheckInDate and CheckOutDate are NOT NULL
                                                                      --(
                                                                        --ra.CheckInDate IS NOT NULL
                                                                        --AND ra.CheckOutDate IS NOT NULL
                                                                        --AND (ra.CheckInDate <= @EndDate AND ra.CheckOutDate >= @StartDate)
                                                                      --)
                                                                      --OR
                                                                      -- Case 2: CheckInDate is NOT NULL, but CheckOutDate is NULL (indefinite occupancy)
                                                                      --(
                                                                        --ra.CheckInDate IS NOT NULL
                                                                        --AND ra.CheckOutDate IS NULL
                                                                        --AND (ra.CheckInDate <= @EndDate)
                                                                      --)
                                                                      --OR
                                                                      -- Case 3: Both CheckInDate and CheckOutDate are NULL (fall back to FD and TD)
                                                                      --(
                                                                        --ra.CheckInDate IS NULL
                                                                        --AND ra.CheckOutDate IS NULL
                                                                        --AND (ra.FD <= @EndDate AND ra.TD >= @StartDate)
                                                                      --)
                                                                    --)
                                                                    AND (
                                                                        @StartDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)  
                                                                        OR 
                                                                        @EndDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)
                                                                        OR
                                                                        ISNULL(CheckInDate, FD) BETWEEN @StartDate AND @EndDate  
                                                                        OR
                                                                        ISNULL(CheckOutDate, TD) BETWEEN @StartDate AND @EndDate
                                                                    )
                                                                )

)
Select ar.RNumber RoomNo,
	isnull((STUFF((
	Select '-' + md.CustomerName from RoomAllocation ra Left Join MembersDetails md on ra.GuestID = md.Id where ra.RNumber= ar.RNumber and ISNULL(ra.Cancelled, 0) = 0 and ar.Rnumber in (
                                                                  SELECT isnull(ra1.RNumber,'')
                                                                  FROM RoomAllocation ra1																  
                                                                  WHERE ra1.IsActive = 1 and ISNULL(ra1.Cancelled, 0) = 0
                                                                    AND (
                                                                        @StartDate BETWEEN ISNULL(ra.CheckInDate, ra.FD) AND ISNULL(ra.CheckOutDate, ra.TD)  
                                                                        OR 
                                                                        @EndDate BETWEEN ISNULL(ra.CheckInDate, ra.FD) AND ISNULL(ra.CheckOutDate, ra.TD)
                                                                        OR
                                                                        ISNULL(ra.CheckInDate, ra.FD) BETWEEN @StartDate AND @EndDate  
                                                                        OR
                                                                        ISNULL(ra.CheckOutDate, ra.TD) BETWEEN @StartDate AND @EndDate
                                                                    )
                                                                    --AND ((ra.CheckInDate IS NOT NULL AND ra.CheckOutDate IS NOT NULL AND (ra.CheckInDate <= @EndDate AND ra.CheckOutDate >= @StartDate))
                                                                      --OR (ra.CheckInDate IS NOT NULL AND ra.CheckOutDate IS NULL AND (ra.CheckInDate <= @EndDate))
                                                                      --OR (ra.CheckInDate IS NULL AND ra.CheckOutDate IS NULL AND (ra.FD <= @EndDate AND ra.TD >= @StartDate))
                                                                    --)
                                                                )
	FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)'), 1, 1, '' )),'') as SharedWith
																
from AvailableRooms ar where ar.RNumber not in (Select isnull(ral.RNumber,'') from RoomAllocation ral where ral.GuestID=@GuestId and ISNULL(ral.Cancelled, 0) = 0)";
            var sParameters = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.GetTableData<AvailableRoomsForGuestAllocation>(sQuery, sParameters);
            return Ok(res);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomsAvailableForGuestsSharedByRoomType(int GuestId, int RoomTypeId)
    {
        try
        {
            string sQuery = @"DECLARE @StartDate DATETIME='',@EndDate DATETIME='',@RTypeID INT=0,@Status INT = 1;
Select @StartDate = (Case when md.DateOfArrival > getdate() then md.DateOfArrival else getdate() end),@EndDate=md.DateOfDepartment from MembersDetails md where Id=@GuestId;
SELECT @RTypeID = @RoomTypeId;
                                                            WITH AvailableRooms AS (
                                                              SELECT r.RNumber
                                                              FROM Rooms r
                                                              WHERE r.RTypeID = @RTypeID
                                                                AND r.Status = @Status
                                                                AND NOT EXISTS (
                                                                  SELECT 1
                                                                  FROM RoomLock rl
                                                                  WHERE rl.Status = 1
                                                                    AND rl.[Type] IN ('Lock', 'Hold')
                                                                    AND (
                                                                          (rl.Rooms IS NOT NULL AND rl.Rooms = r.RNumber)
                                                                       OR (rl.Rooms IS NULL AND rl.RType = r.RTypeID)
                                                                        )
                                                                    AND (
                                                                          (rl.FD IS NULL OR CAST(rl.FD AS DATE) <= CAST(@EndDate AS DATE))
                                                                      AND (rl.ED IS NULL OR CAST(rl.ED AS DATE) >= CAST(@StartDate AS DATE))
                                                                        )
                                                                )
                                                                AND r.RNumber NOT IN (
                                                                  SELECT isnull(ra.RNumber,'')
                                                                  FROM RoomAllocation ra
                                                                  WHERE ra.IsActive = 1 and ISNULL(ra.Cancelled, 0) = 0 and ra.Shared=2
                                                                    AND (
                                                                        @StartDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)  
                                                                        OR 
                                                                        @EndDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)
                                                                        OR
                                                                        ISNULL(CheckInDate, FD) BETWEEN @StartDate AND @EndDate  
                                                                        OR
                                                                        ISNULL(CheckOutDate, TD) BETWEEN @StartDate AND @EndDate
                                                                    )
                                                                )

)
Select ar.RNumber RoomNo,
	isnull((STUFF((
	Select '-' + md.CustomerName from RoomAllocation ra Left Join MembersDetails md on ra.GuestID = md.Id where ra.RNumber= ar.RNumber and ISNULL(ra.Cancelled, 0) = 0 and ar.Rnumber in (
                                                                  SELECT isnull(ra1.RNumber,'')
                                                                  FROM RoomAllocation ra1																  
                                                                  WHERE ra1.IsActive = 1 and ISNULL(ra1.Cancelled, 0) = 0
                                                                    AND (
                                                                        @StartDate BETWEEN ISNULL(ra.CheckInDate, ra.FD) AND ISNULL(ra.CheckOutDate, ra.TD)  
                                                                        OR 
                                                                        @EndDate BETWEEN ISNULL(ra.CheckInDate, ra.FD) AND ISNULL(ra.CheckOutDate, ra.TD)
                                                                        OR
                                                                        ISNULL(ra.CheckInDate, ra.FD) BETWEEN @StartDate AND @EndDate  
                                                                        OR
                                                                        ISNULL(ra.CheckOutDate, ra.TD) BETWEEN @StartDate AND @EndDate
                                                                    )
                                                                )
	FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)'), 1, 1, '' )),'') as SharedWith
																
from AvailableRooms ar where ar.RNumber not in (Select isnull(ral.RNumber,'') from RoomAllocation ral where ral.GuestID=@GuestId and ISNULL(ral.Cancelled, 0) = 0)";
            var sParameters = new { @GuestId = GuestId, @RoomTypeId = RoomTypeId };
            var res = await _unitOfWork.GenOperations.GetTableData<AvailableRoomsForGuestAllocation>(sQuery, sParameters);
            return Ok(res);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomsAvailableForGuestsSharedByRoomType)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomsAvailableForGuestsNonSharedByRoomType(int GuestId, int RoomTypeId)
    {
        try
        {
            string sQuery = @"DECLARE @StartDate DATETIME='',@EndDate DATETIME='',@RTypeID INT=0,@Status INT = 1;
                            Select @StartDate = (Case when md.DateOfArrival > getdate() then md.DateOfArrival else getdate() end),@EndDate=md.DateOfDepartment from MembersDetails md where Id=@GuestId;
                            SELECT @RTypeID = @RoomTypeId;
                                                            WITH AvailableRooms AS (
                                                              SELECT r.RNumber
                                                              FROM Rooms r
                                                              WHERE r.RTypeID = @RTypeID
                                                                AND r.Status = @Status
                                                                AND NOT EXISTS (
                                                                  SELECT 1
                                                                  FROM RoomLock rl
                                                                  WHERE rl.Status = 1
                                                                    AND rl.[Type] IN ('Lock', 'Hold')
                                                                    AND (
                                                                          (rl.Rooms IS NOT NULL AND rl.Rooms = r.RNumber)
                                                                       OR (rl.Rooms IS NULL AND rl.RType = r.RTypeID)
                                                                        )
                                                                    AND (
                                                                          (rl.FD IS NULL OR CAST(rl.FD AS DATE) <= CAST(@EndDate AS DATE))
                                                                      AND (rl.ED IS NULL OR CAST(rl.ED AS DATE) >= CAST(@StartDate AS DATE))
                                                                        )
                                                                )
                                                                AND r.RNumber NOT IN (
                                                                  SELECT isnull(ra.RNumber,'')
                                                                  FROM RoomAllocation ra
                                                                  WHERE ra.IsActive = 1
                                                                    AND ISNULL(ra.Cancelled, 0) = 0
                                                                    AND (
                                                                        @StartDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)  
                                                                        OR 
                                                                        @EndDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)
                                                                        OR
                                                                        ISNULL(CheckInDate, FD) BETWEEN @StartDate AND @EndDate  
                                                                        OR
                                                                        ISNULL(CheckOutDate, TD) BETWEEN @StartDate AND @EndDate
                                                                    )
                                                                )

                                                            )
                                                            Select RNumber RoomNo from AvailableRooms";
            var sParameters = new { @GuestId = GuestId, @RoomTypeId = RoomTypeId };
            var res = await _unitOfWork.GenOperations.GetTableData<AvailableRoomsForGuestAllocation>(sQuery, sParameters);
            return Ok(res);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRoomsAvailableForGuestsNonSharedByRoomType)}");
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
    public async Task<bool> AutoRoomAssign(MembersDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO.GroupId != null)
            {
                string spName = "AutoAssignRoom";
                var param = new { @GroupId = inputDTO.GroupId };
                int affectedRows = await _unitOfWork.GenOperations.ExecuteStoredProcedureAsync(spName, param);
                if (affectedRows > 0)
                {
                    return true;
                }

            }
            return false;
            //return BadRequest("Unable to assign Rooms");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> IsRoomsAvailable(MembersDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.DateOfArrival == null)
                {
                    return BadRequest("Date of arrival is empty");
                }
                if (inputDTO.DateOfDepartment == null)
                {
                    return BadRequest("Date of department is empty");
                }
                if (inputDTO.RoomType == null)
                {
                    return BadRequest("Room Type is empty");
                }
                if (inputDTO.RoomType == 0)
                {
                    return BadRequest("Room Type is empty");
                }
                //                string sQuery = @"DECLARE @StartDate DATETIME=@StartDate1;
                //                                DECLARE @EndDate DATETIME=@EndDate1;
                //                                DECLARE @RTypeID INT=@RTypeID1;
                //                                DECLARE @Status INT = 1;
                //                                WITH AvailableRooms AS (
                //                                  SELECT r.RNumber
                //                                  FROM Rooms r
                //                                  WHERE r.RTypeID = @RTypeID
                //                                    AND r.Status = @Status
                //                                    AND r.RNumber NOT IN (
                //                                      SELECT ra.RNumber
                //                                      FROM RoomAllocation ra
                //                                      WHERE ra.IsActive = 1 and ra.GuestID!=@GuestId
                //                                        AND (
                //    @StartDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)  
                //    OR 
                //    @EndDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)
                //    OR
                //    ISNULL(CheckInDate, FD) BETWEEN @StartDate AND @EndDate  
                //    OR
                //    ISNULL(CheckOutDate, TD) BETWEEN @StartDate AND @EndDate
                //)
                //                                    )

                //                                )
                //                                Select count(1) from AvailableRooms";
                string sQuery = @"DECLARE @StartDate DATETIME=@StartDate1;
                                DECLARE @EndDate DATETIME=@EndDate1;
                                DECLARE @RTypeID INT=@RTypeID1;
                                DECLARE @Status INT = 1;
                                WITH GuestGroup AS (
                                    SELECT GroupId
                                    FROM MembersDetails
                                    WHERE Id = @GuestID
                                ),
                                GroupMembers AS (
                                    SELECT Id AS GuestID
                                    FROM MembersDetails
                                    WHERE GroupId = (SELECT GroupId FROM GuestGroup)
                                ),
                                GuestLastRoom AS (
                                    SELECT TOP 1 RNumber
                                    FROM RoomAllocation
                                    WHERE GuestID in (SELECT Id
                                    FROM MembersDetails
                                    WHERE GroupId = (SELECT GroupId FROM GuestGroup))
                                    AND IsActive = 1
                                    AND ISNULL(Cancelled, 0) = 0
                                    ORDER BY Id DESC
                                ),
                                ExcludedGuestIDs AS (
                                    SELECT ra.GuestID
                                    FROM RoomAllocation ra
                                    WHERE ra.GuestID IN (SELECT GuestID FROM GroupMembers)
                                      AND ra.RNumber = (SELECT RNumber FROM GuestLastRoom)
                                      AND ra.IsActive = 1
                                      AND ISNULL(ra.Cancelled, 0) = 0
                                ),
                                AvailableRooms AS ( 
                                SELECT r.RNumber
                                FROM Rooms r
                                WHERE r.RTypeID = @RTypeID
                                AND r.Status = @Status
                                AND NOT EXISTS (
                                    SELECT 1
                                    FROM RoomLock rl
                                    WHERE rl.Status = 1
                                      AND rl.[Type] IN ('Lock', 'Hold')
                                      AND (
                                            (rl.Rooms IS NOT NULL AND rl.Rooms = r.RNumber)
                                         OR (rl.Rooms IS NULL AND rl.RType = r.RTypeID)
                                          )
                                      AND (
                                            (rl.FD IS NULL OR CAST(rl.FD AS DATE) <= CAST(@EndDate AS DATE))
                                        AND (rl.ED IS NULL OR CAST(rl.ED AS DATE) >= CAST(@StartDate AS DATE))
                                          )
                                )
                                AND r.RNumber NOT IN (
                                SELECT isnull(ra.RNumber,'')
                                FROM RoomAllocation ra
                                WHERE ra.IsActive = 1 and ISNULL(ra.Cancelled, 0) = 0 and ra.GuestID not in (SELECT GuestID FROM ExcludedGuestIDs)
                                AND (
                                    @StartDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)  
                                    OR 
                                    @EndDate BETWEEN ISNULL(CheckInDate, FD) AND ISNULL(CheckOutDate, TD)
                                    OR
                                    ISNULL(CheckInDate, FD) BETWEEN @StartDate AND @EndDate  
                                    OR
                                    ISNULL(CheckOutDate, TD) BETWEEN @StartDate AND @EndDate
                                )))
                                Select count(1) from AvailableRooms";
                var sParam = new { @StartDate1 = inputDTO?.DateOfArrival?.ToString("yyyy-MM-dd hh:mm tt"), @EndDate1 = inputDTO?.DateOfDepartment?.ToString("yyyy-MM-dd hh:mm tt"), @RTypeID1 = inputDTO?.RoomType, @GuestId = inputDTO?.Id };

                int roomsAvailable = await _unitOfWork.GenOperations.GetEntityCount(sQuery, sParam);
                if (roomsAvailable == 0)
                {
                    return BadRequest("No rooms available");
                }
                else if (inputDTO?.NoOfRooms > roomsAvailable)
                {
                    return BadRequest($"Only {roomsAvailable} rooms are available. Try changing dates or Room Type");
                }
                else
                {
                    return Ok($"{roomsAvailable} rooms are available");
                }
            }
            else
            {
                return BadRequest("Data is not valid");
            }

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
            await ValidateBillingForDateChange(inputDTO);


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
                if (true)
                {
                    string query = "Select * from MembersDetails where Id=@Id";
                    var parameter = new { @Id = inputDTO.Id };
                    var memberDetails = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>(query, parameter);

                    if (memberDetails != null)
                    {
                        var roomAllocation = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>(
                            "Select * from RoomAllocation where GuestID=@GuestId and IsActive=1",
                            new { @GuestId = inputDTO.Id });

                        if (roomAllocation?.CheckOutDate != null)
                        {
                            return BadRequest("Guest is already checked out");
                        }

                        var settlementExists = await _unitOfWork.GenOperations.IsExists(
                            "Select 1 from Settlement where IsActive=1 and (GuestId=@GuestId or GuestIdPaxSN1=@GuestId)",
                            new { @GuestId = inputDTO.Id });

                        if (settlementExists)
                        {
                            return BadRequest("Settlement already exists for this guest");
                        }

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
                            inputDTO.opt = "Update";
                            await UpdateRoomAllocation(inputDTO);
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
                if (true)
                {
                    inputDTO.Id = await _unitOfWork.MembersDetails.AddAsync(_mapper.Map<MembersDetails>(inputDTO));
                    if (inputDTO.Id > 0)
                    {
                        inputDTO.opt = "Add";
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

    public async Task<IActionResult> ValidateBillingForDateChange(MembersDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var memberDetails = await _unitOfWork.MembersDetails.GetEntityData<MembersDetailsDTO>("Select * from MembersDetails where GroupId=@GroupId and PAXSno=@PAXSno and Status=1", new { @GroupId = inputDTO.GroupId, @PAXSno = inputDTO.PAXSno });
                if (memberDetails != null)
                {
                    var billingDetails = await _unitOfWork.Billing.GetTableData<BillingDTO>("Select * from Billing where GuestId=@GuestId", new { @GuestId = memberDetails.Id });
                    if (billingDetails != null)
                    {
                        await _unitOfWork.Billing.RunSQLCommand("Update b set b.Confirmed=null from Billing b where b.GuestId = @GuestId ", new { @GuestId = memberDetails.Id });
                    }
                }

                return Ok();
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }



    public async Task<IActionResult> SaveDetailsToEnquiry(GuestReservationDTO inputDTO)
    {
        try
        {

            inputDTO.Id = await _unitOfWork.GuestReservation.AddAsync(_mapper.Map<GuestReservation>(inputDTO));
            if (inputDTO.Id > 0)
            {
                return Ok(inputDTO);
            }
            else
            {
                return BadRequest("");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> UpdateRoomAllocation(MembersDetailsDTO inputDTO)
    {
        try
        {
            string sQuery = "Select * from RoomAllocation where GuestID=@GuestId";
            var sParam = new { @GuestId = inputDTO.Id };

            var ra = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>(sQuery, sParam);
            if (ra != null)
            {
                ra.Rtype = inputDTO.RoomType;
                ra.Rtype = inputDTO.RoomType;
                ra.Fd = inputDTO.DateOfArrival;
                ra.Td = inputDTO.DateOfDepartment;
                ra.ModifiedDate = DateTime.Now;
                await _unitOfWork.RoomAllocation.UpdateAsync(ra);
                return Ok("");
            }

            return BadRequest("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveAttachmentDetailsToDatabase(GuestDocumentAttachmentsDTO inputDTO)
    {
        try
        {
            string eQuery = "";
            if (inputDTO.AttachmentType == "GuestPhoto")
                eQuery = "Select * from GuestDocumentAttachments where isactive=1 and attachmenttype=@attachmenttype and ReferenceId=@ReferenceId";
            else
                eQuery = "Select * from GuestDocumentAttachments where isactive=1 and attachmentname=@attachmentname and attachmenttype=@attachmenttype and ReferenceId=@ReferenceId";
            var eParameters = new { @ReferenceId = inputDTO.ReferenceId, @attachmenttype = inputDTO.AttachmentType, @attachmentname = inputDTO.AttachmentName };

            var exists = await _unitOfWork.GuestDocumentAttachments.IsExists(eQuery, eParameters);
            if (exists)
            {
                if (inputDTO.AttachmentType == "GuestPhoto")
                    return BadRequest("The photo you are trying to upload for this guest is already there. Please delete it and try again");
                else if (inputDTO.AttachmentType == "GuestIdProof")
                    return BadRequest("The Id Proof you are trying to upload for this guest is already there. Please delete it and try again");
                else if (inputDTO.AttachmentType == "GuestPassport")
                    return BadRequest("The Passport you are trying to upload for this guest is already there. Please delete it and try again");
                else if (inputDTO.AttachmentType == "GuestVisa")
                    return BadRequest("The Visa you are trying to upload for this guest is already there. Please delete it and try again");
            }
            else
            {
                inputDTO.Id = await _unitOfWork.GuestDocumentAttachments.AddAsync(_mapper.Map<GuestDocumentAttachments>(inputDTO));
                if (inputDTO.Id > 0)
                {
                    return Ok(inputDTO);
                }
            }
            return BadRequest("Some error occurred while uploading attachment");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomAllocationByGuestId(int GuestId)
    {
        try
        {
            string sQuery = @"Select * from RoomAllocation where GuestID=@GuestId";
            var sParam = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.GetEntityData<RoomAllocationDTO>(sQuery, sParam);
            return Ok(res);
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
            inputDTO.Fd = membersDetails.DateOfArrival;//?.ToString("yyyy-MM-dd HH:mm");
            inputDTO.Td = membersDetails.DateOfDepartment;//?.ToString("yyyy-MM-dd  HH:mm");
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


    public async Task<IActionResult> UpdateCheckInOutDates(RoomAllocationDTO? inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var allocationRoom = await _unitOfWork.RoomAllocation.GetEntityData<RoomAllocation>("Select * from RoomAllocation where GuestID=@GuestID", new { @GuestID = inputDTO.GuestId });

                if (allocationRoom != null)
                {
                    if (inputDTO.CheckInDate != null)
                    {
                        allocationRoom.CheckInDate = inputDTO.CheckInDate;
                    }
                    if (inputDTO.CheckOutDate != null)
                    {
                        allocationRoom.CheckOutDate = inputDTO.CheckOutDate;
                    }
                    allocationRoom.ModifiedBy = inputDTO.ModifiedBy;
                    allocationRoom.ModifiedDate = DateTime.Now;
                    allocationRoom.Reason = allocationRoom.Reason + " || Check in and out Dates Changed by the system ";

                    bool updated = await _unitOfWork.RoomAllocation.UpdateAsync(allocationRoom);

                    if (updated)
                    {
                        return Ok("");
                    }
                    else
                    {
                        return BadRequest("");
                    }
                }
            }
            return BadRequest("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> MarkGuestNoShow(RoomAllocationDTO inputDTO)
    {
        try
        {
            string sQuery = "Select * from MembersDetails where Id=@Id";
            var sParam = new { @Id = inputDTO.GuestId };
            var memberDetails = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>(sQuery, sParam);

            if (memberDetails != null)
            {
                var roomallocation = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>("Select * from RoomAllocation where GuestID=@GuestId", new { @GuestId = inputDTO.GuestId });
                if (roomallocation != null)
                {
                    roomallocation.NoShow = true;
                    roomallocation.CheckInDate = memberDetails.DateOfArrival;
                    roomallocation.CheckOutDate = memberDetails.DateOfArrival;
                    roomallocation.NoShowReason = inputDTO.NoShowReason;
                    var updated = await _unitOfWork.RoomAllocation.UpdateAsync(roomallocation);
                    if (updated)
                    {
                        return Ok("");
                    }
                    else
                    {
                        return BadRequest("Unable to mark no show right now");
                    }
                }
                else
                {
                    return BadRequest("Room details not found");
                }
            }
            else
            {
                return BadRequest("Unable to find the details of the guest");
            }
            return BadRequest("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> GetCancellationVerification(int guestId)
    {
        try
        {
            if (guestId <= 0)
            {
                return BadRequest("Invalid guest details");
            }

            var verification = await _unitOfWork.GuestCancellationVerification.GetEntityData<GuestCancellationVerification>(
                "SELECT TOP 1 * FROM GuestCancellationVerification WHERE GuestId=@GuestId",
                new { @GuestId = guestId });

            var totalAmountPaid = await GetTotalAmountPaidForGuest(guestId);

            if (verification == null)
            {
                return Ok(new GuestCancellationVerificationDTO
                {
                    GuestId = guestId,
                    CancellationAmount = totalAmountPaid,
                    TotalAmountPaid = totalAmountPaid,
                    IsVerified = false
                });
            }

            var response = _mapper.Map<GuestCancellationVerificationDTO>(verification);
            response.TotalAmountPaid = totalAmountPaid;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetCancellationVerification)}");
            throw;
        }
    }

    public async Task<IActionResult> VerifyCancellationAmount(GuestCancellationVerificationDTO inputDTO)
    {
        try
        {
            if (inputDTO == null || inputDTO.GuestId <= 0)
            {
                return BadRequest("Invalid guest details");
            }

            if (inputDTO.CancellationAmount < 0)
            {
                return BadRequest("Cancellation amount cannot be negative");
            }

            var memberDetail = await _unitOfWork.MembersDetails.GetEntityData<MembersDetails>("Select * from MembersDetails where Id=@Id", new { @Id = inputDTO.GuestId });
            if (memberDetail == null)
            {
                return BadRequest("Unable to find the details of the guest");
            }

            var verification = await _unitOfWork.GuestCancellationVerification.GetEntityData<GuestCancellationVerification>(
                "SELECT TOP 1 * FROM GuestCancellationVerification WHERE GuestId=@GuestId",
                new { @GuestId = inputDTO.GuestId });

            var now = DateTime.Now;

            if (verification == null)
            {
                verification = new GuestCancellationVerification
                {
                    GuestId = inputDTO.GuestId,
                    CancellationAmount = inputDTO.CancellationAmount,
                    IsVerified = true,
                    VerifiedOn = now,
                    VerifiedBy = inputDTO.VerifiedBy
                };

                verification.Id = await _unitOfWork.GuestCancellationVerification.AddAsync(verification);
                if (verification.Id <= 0)
                {
                    return BadRequest("Unable to verify the cancellation amount right now");
                }
            }
            else
            {
                verification.CancellationAmount = inputDTO.CancellationAmount;
                verification.IsVerified = true;
                verification.VerifiedOn = now;
                verification.VerifiedBy = inputDTO.VerifiedBy;

                bool isUpdated = await _unitOfWork.GuestCancellationVerification.UpdateAsync(verification);
                if (!isUpdated)
                {
                    return BadRequest("Unable to verify the cancellation amount right now");
                }
            }

            var response = _mapper.Map<GuestCancellationVerificationDTO>(verification);
            response.TotalAmountPaid = await GetTotalAmountPaidForGuest(inputDTO.GuestId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(VerifyCancellationAmount)}");
            throw;
        }
    }

    private async Task<decimal> GetTotalAmountPaidForGuest(int guestId)
    {
        if (guestId <= 0)
        {
            return 0m;
        }

        const string totalPaymentsQuery = @"SELECT ISNULL(SUM(p.Amount), 0) AS TotalAmount
                                            FROM Payment p
                                            WHERE p.IsActive = 1
                                            AND (
                                                p.GuestId = @GuestId
                                                OR p.GuestId IN (
                                                    SELECT DISTINCT md.Id
                                                    FROM MembersDetails md
                                                    LEFT JOIN RoomAllocation ra ON md.Id = ra.GuestID
                                                    WHERE md.GroupId = (
                                                        SELECT GroupId
                                                        FROM MembersDetails
                                                        WHERE Id = @GuestId
                                                    )
                                                    AND ra.RNumber IN (
                                                        SELECT RNumber
                                                        FROM RoomAllocation
                                                        WHERE GuestID = @GuestId
                                                    )
                                                )
                                            )";

        var totalPayments = await _unitOfWork.GenOperations.GetEntityData<BillingDTO>(totalPaymentsQuery, new { @GuestId = guestId });
        var totalAmountPaid = totalPayments?.TotalAmount ?? 0d;

        return Convert.ToDecimal(totalAmountPaid);
    }

    public async Task<IActionResult> CancelGuestReservation(RoomAllocationDTO inputDTO)
    {
        try
        {
            if (inputDTO.GuestId == null)
            {
                return BadRequest("Invalid guest details");
            }

            var memberDetail = await _unitOfWork.GenOperations.GetEntityData<MembersDetails>("Select * from MembersDetails where Id=@Id", new { @Id = inputDTO.GuestId });
            if (memberDetail == null)
            {
                return BadRequest("Unable to find the details of the guest");
            }

            if (memberDetail.Status != 1)
            {
                return BadRequest("Reservation cannot be cancelled in the current status");
            }

            string? cancellationReason = !string.IsNullOrWhiteSpace(inputDTO.CancelledReason)
                ? inputDTO.CancelledReason
                : inputDTO.Reason;

            if (string.IsNullOrWhiteSpace(cancellationReason))
            {
                return BadRequest("Please provide a cancellation reason");
            }

            if (inputDTO.ModeOfCancellation == null || inputDTO.ModeOfCancellation <= 0)
            {
                return BadRequest("Please select a mode of cancellation");
            }

            if (string.IsNullOrWhiteSpace(inputDTO.CancellationRequestedBy))
            {
                return BadRequest("Please specify who requested the cancellation");
            }

            var verification = await _unitOfWork.GuestCancellationVerification.GetEntityData<GuestCancellationVerification>(
                "SELECT TOP 1 * FROM GuestCancellationVerification WHERE GuestId=@GuestId",
                new { @GuestId = inputDTO.GuestId });

            if (verification == null || !verification.IsVerified)
            {
                return BadRequest("Please verify the refund amount before cancelling this reservation");
            }

            var roomAllocation = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>("Select * from RoomAllocation where GuestID=@GuestId and IsActive=1", new { @GuestId = inputDTO.GuestId });
            bool isExistingAllocation = roomAllocation != null;

            if (!isExistingAllocation)
            {
                roomAllocation = new RoomAllocation
                {
                    GuestId = inputDTO.GuestId,
                    Fd = memberDetail.DateOfArrival,
                    Td = memberDetail.DateOfDepartment,
                    AsigndDate = DateTime.Now,
                    IsActive = 1,
                    CreeatedBy = inputDTO.CancelledBy,
                    Rtype = memberDetail.RoomType
                };
            }
            else if (roomAllocation.Cancelled == true)
            {
                return BadRequest("Reservation has already been cancelled");
            }

            string cancellationId = roomAllocation.CancellationId ?? await GenerateUniqueCancellationId();
            DateTime cancellationTimestamp = DateTime.Now;

            roomAllocation.Cancelled = true;
            roomAllocation.CancelledReason = cancellationReason;
            roomAllocation.ModeOfCancellation = inputDTO.ModeOfCancellation;
            roomAllocation.CancelledBy = inputDTO.CancelledBy;
            roomAllocation.CancelledOn = cancellationTimestamp;
            roomAllocation.CancellationId = cancellationId;
            roomAllocation.CancellationRequestedBy = inputDTO.CancellationRequestedBy?.Trim();
            roomAllocation.ModifiedBy = inputDTO.CancelledBy ?? roomAllocation.ModifiedBy;
            roomAllocation.ModifiedDate = cancellationTimestamp;

            if (roomAllocation.Fd == null)
            {
                roomAllocation.Fd = memberDetail.DateOfArrival;
            }

            if (roomAllocation.Td == null)
            {
                roomAllocation.Td = memberDetail.DateOfDepartment;
            }

            if (roomAllocation.AsigndDate == null)
            {
                roomAllocation.AsigndDate = cancellationTimestamp;
            }

            if (roomAllocation.IsActive == null)
            {
                roomAllocation.IsActive = 1;
            }

            if (roomAllocation.Rtype == null)
            {
                roomAllocation.Rtype = memberDetail.RoomType;
            }

            if (isExistingAllocation)
            {
                bool allocationUpdated = await _unitOfWork.RoomAllocation.UpdateAsync(roomAllocation);
                if (!allocationUpdated)
                {
                    return BadRequest("Unable to cancel reservation right now");
                }
            }
            else
            {
                roomAllocation.Id = await _unitOfWork.RoomAllocation.AddAsync(roomAllocation);
                if (roomAllocation.Id <= 0)
                {
                    return BadRequest("Unable to cancel reservation right now");
                }
            }

            return Ok(new
            {
                CancellationId = cancellationId,
                BookingId = memberDetail.GroupId,
                CancelledOn = cancellationTimestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CancelGuestReservation)}");
            throw;
        }
    }

    private async Task<string> GenerateUniqueCancellationId()
    {
        const string prefix = "CANC";
        const int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int randomNumber = RandomNumberGenerator.GetInt32(0, 100_000_000);
            string candidate = $"{prefix}{randomNumber:D8}";

            var existingCount = await _unitOfWork.GenOperations.GetEntityCount(
                "SELECT COUNT(1) FROM RoomAllocation WHERE CancellationId = @CancellationId",
                new { CancellationId = candidate });

            if (existingCount == 0)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Unable to generate a unique cancellation ID.");
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

    public async Task<bool> CheckMandatoryCheckList(MedicalSoultion_GuestCheckList inputDTO, string CheckListType)
    {
        int[] mandatoryFields = Array.Empty<int>(); // Ensure it's initialized
        int[] fieldsFromPage = Array.Empty<int>();  // Ensure it's initialized

        string squery = "Select * from TblCheckLists where ChecklistType=@CheckListType and IsMandatory=1 and IsActive=1";
        var sparam = new { @CheckListType = CheckListType };

        // Fetch mandatory checklist IDs from the database
        var res = await _unitOfWork.GenOperations.GetTableData<TblCheckListsDTO>(squery, sparam);
        if (res != null && res.Any())
        {
            mandatoryFields = res.Select(x => x.ID).ToArray();
        }

        // Fetch checklist IDs from inputDTO
        if (inputDTO != null && !string.IsNullOrEmpty(inputDTO.checklist))
        {
            fieldsFromPage = inputDTO.checklist.Split(',')
                          .Select(s => int.TryParse(s, out int num) ? num : (int?)null)
                          .Where(num => num.HasValue)
                          .Select(num => num.Value)
                          .ToArray();
        }
        if (mandatoryFields.Length > 0 && fieldsFromPage != null)
        {
            // Check if all mandatory checklist items are present in fieldsFromPage
            bool allPresent = mandatoryFields.Length > 0 && fieldsFromPage.Length > 0 &&
                              mandatoryFields.All(id => fieldsFromPage.Contains(id));

            if (allPresent)
            {
                return true;
                //Console.WriteLine("✅ All mandatory checklist items are present.");
            }
            //else
            //{
            //    Console.WriteLine("❌ Some mandatory checklist items are missing.");
            //}
        }
        return false;
    }
    public async Task<bool> GuestCheckInValidateEarlyCheckIn(RoomAllocation guestsCheckinDetails)
    {
        if (guestsCheckinDetails != null && guestsCheckinDetails.Fd != null)
        {
            // Convert expected check-out time to DateTime
            //if (!DateTime.TryParse(guestsCheckinDetails.Fd, out DateTime expectedCheckinTime))
            //{
            //    return true; // Invalid date format, cannot proceed.
            //}
            DateTime expectedCheckinTime = guestsCheckinDetails.Fd ?? default(DateTime);
            // Get the current time in Indian Standard Time (IST)
            TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime actualDateTimeIST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);

            //string errorPath = Path.Combine(_hostingEnv.WebRootPath, "LogError");

            //CommonHelper.WriteToFile(errorPath, "Log.txt", actualDateTimeIST.ToString("dd-MMM-yyyy hh:mm tt"));
            actualDateTimeIST = actualDateTimeIST.AddHours(3);
            bool isEarlyCheckIn = expectedCheckinTime > actualDateTimeIST.AddHours(3);
            //CommonHelper.WriteToFile(errorPath, "Log.txt", expectedCheckinTime + "  :  " + actualDateTimeIST.ToString("dd-MMM-yyyy hh:mm tt"));
            //CommonHelper.WriteToFile(errorPath, "Log.txt", "EarlyCheckin : " + isEarlyCheckIn);
            // Check if the guest is checking out at least 3 hours early
            if (isEarlyCheckIn)
            {
                //CommonHelper.WriteToFile(errorPath, "Log.txt", "returned early check in as true");
                return true; // Early checkout
            }
        }
        return false; // Not an early checkout

        //if (guestsCheckinDetails != null)
        //{
        //    string? expectedCheckin = guestsCheckinDetails.Td;
        //    //Convert it to datetime.
        //    DateTime actualDateTime = System.DateTime.Now; //This should be indian Standard time 

        //    //Check if guest is Checking 3 hours early

        //    //if early checkin then true else false return;

        //}
        //return false;
    }

    public async Task<bool> CheckRoomTidyStatusByRoomNo(string RoomNo)
    {
        try
        {
            string sQuery = @"
            SELECT 
                CASE 
                    WHEN r.RoomClean = 1 AND rclLatest.ChkDate IS NOT NULL AND DATEDIFF(HOUR, rclLatest.ChkDate, GETDATE()) <= 24 THEN 1
                    ELSE 0
                END AS IsCleaned
            FROM Rooms r
            OUTER APPLY (
                SELECT TOP 1 ChkDate
                FROM RoomChkList rcl
                WHERE rcl.RID = r.ID
                ORDER BY ChkDate DESC
            ) rclLatest
            WHERE r.RNumber = @RoomNumber";

            var sParam = new { RoomNumber = RoomNo };

            var res = await _unitOfWork.TaskMaster.GetEntityData<dynamic>(sQuery, sParam);

            return res?.IsCleaned == 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving tidy status for room {RoomNo} in {nameof(CheckRoomTidyStatusByRoomNo)}");
            throw;
        }
    }

    public async Task<bool> CheckRoomTidyStatusByGuestId(int GuestId)
    {
        try
        {
            string sQuery = @"Select * from RoomAllocation where GuestID=@GuestID and IsActive=1";
            var rooms = await _unitOfWork.RoomAllocation.GetEntityData<RoomAllocation>(sQuery, new { @GuestID = GuestId });
            if (rooms != null)
            {
                return await CheckRoomTidyStatusByRoomNo(rooms.Rnumber ?? "");
            }
            return false;
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
            if (await CheckRoomTidyStatusByGuestId(inputDTO.ID) == false)
            {
                return StatusCode(432, "Early check-in is not permitted. Please adjust your check-in time to proceed.");
            }

            string eQuery = "Select * from roomallocation where GuestID=@GuestID and IsActive=1";
            var eParam = new { @GuestID = inputDTO.ID };

            var eCheckInExists = await _unitOfWork.GenOperations.IsExists(eQuery, eParam);
            bool allMandatoryFieldsChecked = await CheckMandatoryCheckList(inputDTO, "CheckIn");
            if (eCheckInExists && allMandatoryFieldsChecked)
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
                        //Validate For Early Checkin
                        bool GuestCheckinEarly = await GuestCheckInValidateEarlyCheckIn(guestsCheckinDetails);
                        if (GuestCheckinEarly)
                        {
                            return StatusCode(431, "Early check-in is not permitted. Please adjust your check-in time to proceed.");
                        }

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
                if (!eCheckInExists)
                {
                    return BadRequest("Please assign a room to this guest first");
                }
                else if (!allMandatoryFieldsChecked)
                {
                    return StatusCode(430, new { message = "❌ Please review the action items as we need to validate a few before check-in." });
                    //return BadRequest("❌ Some mandatory checklist items are missing.");
                }
                return BadRequest("Please assign a room to this guest first");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<bool> GuestCheckOutValidateEarlyCheckOut(RoomAllocation guestsCheckinDetails)
    {
        if (guestsCheckinDetails != null && guestsCheckinDetails.Td != null)
        {
            // Convert expected check-out time to DateTime
            //if (!DateTime.TryParse(guestsCheckinDetails.Td, out DateTime expectedCheckoutTime))
            //{
            //    return false; // Invalid date format, cannot proceed.
            //}
            DateTime expectedCheckoutTime = guestsCheckinDetails.Td ?? default(DateTime);
            // Get the current time in Indian Standard Time (IST)
            TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime actualDateTimeIST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);

            // Check if the guest is checking out at least 3 hours early
            if (expectedCheckoutTime > actualDateTimeIST.AddHours(3))
            {
                return true; // Early checkout
            }
        }

        return false; // Not an early checkout

        //if (guestsCheckinDetails != null)
        //{
        //    string? expectedCheckin = guestsCheckinDetails.Td;
        //    //Convert it to datetime.
        //    DateTime actualDateTime = System.DateTime.Now; //This should be indian Standard time 

        //    //Check if guest is Checking 3 hours early

        //    //if early checkin then true else false return;

        //}
        //return false;
    }

    public async Task<IActionResult> GuestCheckOut(MedicalSoultion_GuestCheckList inputDTO)
    {
        try
        {
            string eQuery = "Select * from roomallocation where GuestID=@GuestID and IsActive=1";
            var eParam = new { @GuestID = inputDTO.ID };

            var eCheckInExists = await _unitOfWork.GenOperations.IsExists(eQuery, eParam);
            bool allMandatoryFieldsChecked = await CheckMandatoryCheckList(inputDTO, "CheckOut");
            if (eCheckInExists && allMandatoryFieldsChecked)
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
                        //Validate For Early Checkin
                        //bool GuestCheckoutEarly = await GuestCheckOutValidateEarlyCheckOut(guestsCheckinDetails);
                        //if (GuestCheckoutEarly)
                        //{
                        //    inputDTO.EarlyCheckOut = true;
                        //    return Ok(inputDTO);
                        //}

                        guestsCheckinDetails.CheckOutDate = DateTime.Now;
                        guestsCheckinDetails.Reason = inputDTO.Reason;
                        //guestsCheckinDetails.Td = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
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
                if (!eCheckInExists)
                {
                    return BadRequest("Please assign a room to this guest first");
                }
                else if (!allMandatoryFieldsChecked)
                {
                    return StatusCode(430, new { message = "❌ Please review the action items as we need to validate a few before check-out." });
                    //return BadRequest("❌ Some mandatory checklist items are missing.");
                }
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
            string sQuery = "Select * from TaskMaster where IsDeleted=0 and IsActive=1 and isnull(Readonly,0)=0 order by Taskname asc";
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
    public async Task<IActionResult> GetTaskByTaskId(int TaskId)
    {
        try
        {
            string sQuery = "Select * from TaskMaster where Id=@TaskId";
            var sParam = new { @TaskId = TaskId };
            var res = await _unitOfWork.GenOperations.GetEntityData<TaskMasterDTO>(sQuery, sParam);
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
                                where Replace(MobileNo,' ','') like Replace('%'+@Phno+'%',' ','') order by id desc";
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

    public async Task<IActionResult> GetGuestListSharingRoomByGuestId(int GuestId)
    {
        try
        {

            string sQuery = @"Declare @RoomNumber nvarchar(5)= (Select RNumber from RoomAllocation ra where GuestID=@GuestID);
Select md.CustomerName from RoomAllocation ra 
Left Join MembersDetails md on ra.GuestID=md.Id
where GuestID in (Select Id from MembersDetails where GroupId = (Select GroupId from MembersDetails where Id=@GuestID)) and RNumber=@RoomNumber and GuestID!=@GuestID";
            var sParam = new { @GuestID = GuestId };
            var res = await _unitOfWork.MembersDetails.GetTableData<MembersDetailsDTO>(sQuery, sParam);
            return Ok(res);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> AllocateRoomToAllGroup(RoomAllocationDTO inputDTO)
    {
        try
        {
            // Update RoomType in MembersDetails for all group members
            if (inputDTO.Rtype > 0)
            {
                string updateMembersQuery = @"Update md set md.RoomType=@RoomTypeId from MembersDetails md where md.GroupId=(Select GroupId from MembersDetails where Id=@GuestID)";
                var updateMembersParam = new { @GuestID = inputDTO.GuestId, @RoomTypeId = inputDTO.Rtype };
                await _unitOfWork.GenOperations.ExecuteQuery(updateMembersQuery, updateMembersParam);
            }

            // Update RoomAllocation for all group members
            string sQuery = @"Update ra set ra.RNumber=@RoomNumber, ra.Rtype=@RoomTypeId, ra.Shared=1 from RoomAllocation ra where ra.GuestID in ((Select Id from MembersDetails where GroupId=(Select GroupId from MembersDetails where Id=@GuestID)))";
            var sParam = new { @GuestID = inputDTO.GuestId, @RoomNumber = inputDTO.Rnumber, @RoomTypeId = inputDTO.Rtype ?? 0 };
            await _unitOfWork.GenOperations.ExecuteQuery(sQuery, sParam);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AllocateRoomToAllGroup)}");
            throw;
        }
    }
    public async Task<IActionResult> ChangeRoomForCurrentGuestWithSharingStatus(RoomAllocationDTO inputDTO)
    {
        try
        {
            // Update RoomType in MembersDetails
            if (inputDTO.Rtype > 0)
            {
                string updateMembersQuery = @"Update MembersDetails set RoomType=@RoomTypeId where Id=@GuestId";
                var updateMembersParam = new { @GuestId = inputDTO.GuestId, @RoomTypeId = inputDTO.Rtype };
                await _unitOfWork.GenOperations.ExecuteQuery(updateMembersQuery, updateMembersParam);
            }

            string sQuery = @"Select * from RoomAllocation where GuestID=@GuestId";
            var sParam = new { @GuestID = inputDTO.GuestId };

            var ra = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>(sQuery, sParam);
            if (ra != null)
            {
                ra.Rnumber = inputDTO.Rnumber;
                ra.Rtype = inputDTO.Rtype ?? ra.Rtype;
                ra.Shared = 1;
                await _unitOfWork.RoomAllocation.UpdateAsync(ra);
                return Ok();
            }
            else
            {
                var mem = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>("Select * from MembersDetails where id=@Id", new { @Id = inputDTO.GuestId });

                RoomAllocation roomAllocation = new RoomAllocation();
                roomAllocation.Rnumber = inputDTO.Rnumber;
                roomAllocation.Rtype = inputDTO.Rtype ?? mem?.RoomType ?? 0;
                roomAllocation.GuestId = inputDTO.GuestId;
                roomAllocation.Fd = mem?.DateOfArrival;
                roomAllocation.Td = mem?.DateOfDepartment;
                roomAllocation.AsigndDate = DateTime.Now;
                roomAllocation.IsActive = 1;
                roomAllocation.Shared = inputDTO.Shared;
                roomAllocation.Id = await _unitOfWork.RoomAllocation.AddAsync(roomAllocation);
                if (roomAllocation.Id > 0)
                {
                    return Ok("");
                }
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ChangeRoomForCurrentGuestWithSharingStatus)}");
            throw;
        }
    }
    public async Task<IActionResult> ChangeRoomForCurrentGuestWithNonSharingStatus(RoomAllocationDTO inputDTO)
    {
        try
        {
            // Update RoomType in MembersDetails
            if (inputDTO.Rtype > 0)
            {
                string updateMembersQuery = @"Update MembersDetails set RoomType=@RoomTypeId where Id=@GuestId";
                var updateMembersParam = new { @GuestId = inputDTO.GuestId, @RoomTypeId = inputDTO.Rtype };
                await _unitOfWork.GenOperations.ExecuteQuery(updateMembersQuery, updateMembersParam);
            }

            string sQuery = @"Select * from RoomAllocation where GuestID=@GuestId";
            var sParam = new { @GuestID = inputDTO.GuestId };

            var ra = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>(sQuery, sParam);
            if (ra != null)
            {
                ra.Rnumber = inputDTO.Rnumber;
                ra.Rtype = inputDTO.Rtype ?? ra.Rtype;
                ra.Shared = 2;
                await _unitOfWork.RoomAllocation.UpdateAsync(ra);
                return Ok();
            }
            else
            {
                sQuery = "Select * from MembersDetails where Id=@Id";
                var memberDetails = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>(sQuery, new { @Id = inputDTO.GuestId });
                if (memberDetails != null)
                {
                    RoomAllocation roomAllocation = new RoomAllocation();
                    roomAllocation.Rnumber = inputDTO.Rnumber;
                    roomAllocation.Rtype = inputDTO.Rtype ?? memberDetails.RoomType ?? 0;
                    roomAllocation.GuestId = memberDetails.Id;
                    roomAllocation.Fd = memberDetails.DateOfArrival;
                    roomAllocation.Td = memberDetails.DateOfDepartment;
                    roomAllocation.AsigndDate = System.DateTime.Now;
                    roomAllocation.IsActive = 1;
                    roomAllocation.Shared = inputDTO.Shared;
                    roomAllocation.Id = await _unitOfWork.RoomAllocation.AddAsync(roomAllocation);
                    if (roomAllocation.Id > 0)
                    {
                        return Ok("");
                    }
                }
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ChangeRoomForCurrentGuestWithNonSharingStatus)}");
            throw;
        }
    }
    public async Task<IActionResult> FetchState(TblCountriesDTO inputDTO)
    {
        try
        {
            string sQuery = "Select Id,name from states where Country_id=@countryid";
            var sParam = new { @countryid = inputDTO.Id };
            var res = await _unitOfWork.GenOperations.GetTableData<TblStateDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> FetchCity(TblStateDTO inputDTO)
    {
        try
        {
            string sQuery = "Select Id,name from cities where state_id=@stateid";
            var sParam = new { @stateid = inputDTO.Id };
            var res = await _unitOfWork.GenOperations.GetTableData<tblCityDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteGuestAttachmentById(int Id)
    {
        try
        {
            GuestDocumentAttachments dto = await _unitOfWork.GuestDocumentAttachments.FindByIdAsync(Id);
            if (dto != null)
            {
                dto.IsActive = false;
                await _unitOfWork.GuestDocumentAttachments.UpdateAsync(dto);
                return Ok("Save");
            }
            else
            {
                return BadRequest("File not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteGuestAttachmentById)}");
            throw;
        }
    }
    public async Task<IActionResult> GuestAttachmentByAttachmentId(int Id)
    {
        try
        {

            GuestDocumentAttachmentsDTO dto = _mapper.Map<GuestDocumentAttachmentsDTO>(await _unitOfWork.GuestDocumentAttachments.FindByIdAsync(Id));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GuestAttachmentByAttachmentId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetServicesForBilling(int GuestId)
    {
        try
        {

            string query1 = @"Select * from TaskMaster where IsActive=1 and IsDeleted=0 and isnull(Readonly,0)=0";
            var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.TaskMaster.GetTableData<TaskMasterDTO>(query1);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPackagesForBilling(int GuestId)
    {
        try
        {

            string query1 = @"Select * from Services where Status=1 order by Service asc";
            var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.GenderMaster.GetTableData<ServicesDTO>(query1);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetTasksForBilling(int GuestId)
    {
        try
        {

            string query1 = @"Select * from TaskMaster where IsActive=1 and IsDeleted=0 and isnull(Readonly,0)=0 order by TaskName asc";
            var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.GenOperations.GetTableData<TaskMasterDTO>(query1);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPaymentMethodForBilling()
    {
        try
        {
            string query = @"Select Id,PaymentMethodName Code from PaymentMethod where IsActive=1";
            //var parameter = new { @Id = GuestId };
            var res = await _unitOfWork.GenOperations.GetTableData<GuaranteeCodeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveBillingData(BillingViewModel inputDTO, int loginId)
    {
        try
        {
            bool error = false;
            if (inputDTO != null && inputDTO.BillingDTOs != null)
            {
                foreach (var item in inputDTO.BillingDTOs)
                {
                    if (item.Id > 0)
                    {
                        string sQuery = "Select * from Billing where Id=@Id";
                        var sParam = new { @Id = item.Id };
                        var billingData = await _unitOfWork.Billing.GetEntityData<Billing>(sQuery, sParam);
                        if (billingData != null)
                        {
                            billingData.ModifiedBy = loginId;
                            billingData.ModifiedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            billingData.ServiceId = item.ServiceId;
                            billingData.ServiceType = item.ServiceType;
                            billingData.GuestId = item.GuestId;
                            billingData.Count = item.Count;
                            billingData.Price = item.Price;
                            billingData.Discount = item.Discount;
                            billingData.DiscountCode = item.DiscountCode;
                            billingData.HSN_SAC = item.HSN_SAC;
                            billingData.IGST = item.IGST;
                            billingData.SGST = item.SGST;
                            billingData.CGST = item.CGST;
                            billingData.TotalAmount = item.TotalAmount;
                            billingData.BillingTo = item.BillingTo;
                            billingData.BillingMobile = item.BillingMobile;
                            billingData.BillingEmail = item.BillingEmail;
                            billingData.BillingAddressLine1 = item.BillingAddressLine1;
                            billingData.BillingAddressLine2 = item.BillingAddressLine2;
                            var updated = await _unitOfWork.Billing.UpdateAsync(billingData);
                            if (updated == false)
                            {
                                error = true;
                            }
                        }
                    }
                    else
                    {
                        string eQuery = "Select * from Billing where IsActive=1 and ServiceId=@ServiceId and ServiceType=@ServiceType and GuestId=@GuestId";
                        if (item.ServiceType == "PackageSystem")
                        {
                            eQuery = "Select * from Billing where IsActive=1 and ServiceType=@ServiceType and GuestId=@GuestId";
                        }
                        var eParam = new { @ServiceId = item.ServiceId, @ServiceType = item.ServiceType, @GuestId = item.GuestId };
                        var exists = await _unitOfWork.Billing.IsExists(eQuery, eParam);
                        if (exists)
                        {
                            string sQuery = "Select * from Billing where IsActive=1 and ServiceId=@ServiceId and ServiceType=@ServiceType and GuestId=@GuestId";
                            var sParam = new { @ServiceId = item.ServiceId, @ServiceType = item.ServiceType };
                            var billingData = await _unitOfWork.Billing.GetEntityData<Billing>(eQuery, eParam);
                            if (billingData != null)
                            {
                                if (billingData.Confirmed != true)
                                {
                                    billingData.ModifiedBy = loginId;
                                    billingData.ModifiedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                                    billingData.ServiceId = item.ServiceId;
                                    billingData.ServiceType = item.ServiceType;
                                    billingData.GuestId = item.GuestId;
                                    billingData.Count = item.Count;
                                    billingData.Price = item.Price;
                                    billingData.Discount = item.Discount;
                                    billingData.DiscountCode = item.DiscountCode;
                                    billingData.HSN_SAC = item.HSN_SAC;
                                    billingData.IGST = item.IGST;
                                    billingData.SGST = item.SGST;
                                    billingData.CGST = item.CGST;
                                    billingData.TotalAmount = item.TotalAmount;
                                    billingData.BillingTo = item.BillingTo;
                                    billingData.BillingMobile = item.BillingMobile;
                                    billingData.BillingEmail = item.BillingEmail;
                                    billingData.BillingAddressLine1 = item.BillingAddressLine1;
                                    billingData.BillingAddressLine2 = item.BillingAddressLine2;
                                    var updated = await _unitOfWork.Billing.UpdateAsync(billingData);
                                    if (updated == false)
                                    {
                                        error = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.CreatedBy = loginId;
                            item.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            item.IsActive = true;
                            item.Id = await _unitOfWork.Billing.AddAsync(_mapper.Map<Billing>(item));
                            if (item.Id == 0)
                            {
                                error = true;
                            }
                        }
                    }
                }

                if (error == false)
                {

                    return Ok("Save");
                }
                else
                {
                    return BadRequest("Data saved with errors.");
                }
            }
            return BadRequest("Some error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }

    public async Task<IActionResult> ConfirmAndSaveBillingData(BillingDTO inputDTO, int loginId)
    {
        try
        {
            bool error = false;
            if (inputDTO != null)
            {
                if (inputDTO.Id > 0)
                {
                    string sQuery = "Select * from Billing where Id=@Id";
                    var sParam = new { @Id = inputDTO.Id };
                    var billingData = await _unitOfWork.Billing.GetEntityData<Billing>(sQuery, sParam);
                    if (billingData != null)
                    {
                        billingData.ModifiedBy = loginId;
                        billingData.ModifiedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        billingData.ServiceId = inputDTO.ServiceId;
                        billingData.ServiceType = inputDTO.ServiceType;
                        billingData.GuestId = inputDTO.GuestId;
                        billingData.Count = inputDTO.Count;
                        billingData.Price = inputDTO.Price;
                        billingData.Discount = inputDTO.Discount;
                        billingData.HSN_SAC = inputDTO.HSN_SAC;
                        billingData.IGST = inputDTO.IGST;
                        billingData.SGST = inputDTO.SGST;
                        billingData.CGST = inputDTO.CGST;
                        billingData.TotalAmount = inputDTO.TotalAmount;
                        billingData.Confirmed = true;
                        var updated = await _unitOfWork.Billing.UpdateAsync(billingData);
                        if (updated == false)
                        {
                            error = true;
                        }
                    }
                }
                else
                {
                    string eQuery = "Select * from Billing where IsActive=1 and ServiceId=@ServiceId and ServiceType=@ServiceType and GuestId=@GuestId";
                    var eParam = new { @ServiceId = inputDTO.ServiceId, @ServiceType = inputDTO.ServiceType, @GuestId = inputDTO.GuestId };
                    var exists = await _unitOfWork.Billing.IsExists(eQuery, eParam);
                    if (exists)
                    {
                        string sQuery = "Select * from Billing where IsActive=1 and ServiceId=@ServiceId and ServiceType=@ServiceType and GuestId=@GuestId";
                        var sParam = new { @ServiceId = inputDTO.ServiceId, @ServiceType = inputDTO.ServiceType };
                        var billingData = await _unitOfWork.Billing.GetEntityData<Billing>(eQuery, eParam);
                        if (billingData != null)
                        {
                            if (billingData.Confirmed != true)
                            {
                                billingData.ModifiedBy = loginId;
                                billingData.ModifiedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                                billingData.ServiceId = inputDTO.ServiceId;
                                billingData.ServiceType = inputDTO.ServiceType;
                                billingData.GuestId = inputDTO.GuestId;
                                billingData.Count = inputDTO.Count;
                                billingData.Price = inputDTO.Price;
                                billingData.Discount = inputDTO.Discount;
                                billingData.HSN_SAC = inputDTO.HSN_SAC;
                                billingData.IGST = inputDTO.IGST;
                                billingData.SGST = inputDTO.SGST;
                                billingData.CGST = inputDTO.CGST;
                                billingData.TotalAmount = inputDTO.TotalAmount;
                                billingData.Confirmed = true;
                                var updated = await _unitOfWork.Billing.UpdateAsync(billingData);
                                if (updated == false)
                                {
                                    error = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        inputDTO.CreatedBy = loginId;
                        inputDTO.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        inputDTO.IsActive = true;
                        inputDTO.Confirmed = true;
                        inputDTO.Id = await _unitOfWork.Billing.AddAsync(_mapper.Map<Billing>(inputDTO));
                        if (inputDTO.Id == 0)
                        {
                            error = true;
                        }
                    }

                }





                if (error == false)
                {
                    return Ok("Save");
                }
                else
                {
                    return BadRequest("Data saved with errors.");
                }
            }
            return BadRequest("Some error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetGuestsList)}");
            throw;
        }
    }

    public async Task<bool> PostChargesToAuditBySP(int GuestId)
    {
        try
        {
            string sp = "InsertAuditRevenueForGuest";
            var param = new DynamicParameters();
            param.Add("@GuestId", GuestId);
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var res = await _unitOfWork.GMSFinalGuest.ExecuteStoredProcedureAsync(sp, param);
            //await DbConnection.ExecuteAsync("RunNightAudit", p, commandType: CommandType.StoredProcedure);
            int result = param.Get<int>("@Status");
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PostChargesToAuditBySP)}");
            throw;
        }
    }

    public async Task<bool> PostRoomChargesToAudit(BillingDTO billingDTO)
    {
        try
        {
            var inputDTO = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>("Select * from MembersDetails where Id=@Id", new { @Id = billingDTO.GuestId });

            if (inputDTO.GroupId != null)
            {
                string rQuery = "Select RNumber from RoomAllocation where GuestID in (Select Id from MembersDetails where GroupId=@GroupId) Group By RNumber";
                var rooms = await _unitOfWork.GenOperations.GetTableData<string>(rQuery, new { @GroupId = inputDTO.GroupId });

                string mdQuery = "Select * from MembersDetails where GroupId=@GroupId and PAxSno=1";
                var members = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>(mdQuery, new { @GroupId = inputDTO.GroupId });

                string ratesQuery1 = @"SELECT 
                            Distinct
                            r.RoomTypeId AS RoomTypeId,
                            r.[Date] AS RateDate,
                            r.Price AS Rate,
                            s.ID AS PlanId,
                            s.Service AS PlanName,
                            s.Description AS PlanDescription,
                            rt.RType RoomType,
	                        rt.Remarks RoomDescription
                        FROM Rates r
                        LEFT JOIN Services s ON r.PlanId = s.ID
                        LEFT JOIN RoomType rt ON r.RoomTypeId = rt.ID						
                        where r.PlanId=@PlanId 
						and r.RoomTypeId=@RoomTypeId
						and 
						r.[Date] >= 
						CAST(	@DateOfArrival	AS DATE) 
						AND r.[Date] < DATEADD(DAY, Cast( @ServiceId as int ), CAST( @DateOfArrival AS DATE))";
                string ratesQuery = @"WITH DateRange AS (
    SELECT @DateOfArrival AS RateDate
    UNION ALL
    SELECT DATEADD(DAY, 1, RateDate)
    FROM DateRange
    WHERE DATEADD(DAY, 1, RateDate) < DATEADD(DAY, @ServiceId, @DateOfArrival)
),
MatchedRates AS (
    SELECT 
        d.RateDate,
        r.RoomTypeId,
        r.PlanId,
        r.[Date] AS RateDateReal,
        r.Price AS Rate,
        s.ID AS ServiceId,
        s.Service AS PlanName,
        s.Description AS PlanDescription,
        rt.RType AS RoomType,
        rt.Remarks AS RoomDescription
    FROM DateRange d
    CROSS APPLY (
        SELECT TOP 1 *
        FROM Rates r
        WHERE 
            r.RoomTypeId = @RoomTypeId AND
            r.PlanId = @PlanId AND
            r.[Date] <= d.RateDate
        ORDER BY r.[Date] DESC
    ) r
    LEFT JOIN Services s ON r.PlanId = s.ID
    LEFT JOIN RoomType rt ON r.RoomTypeId = rt.ID
)
SELECT 
    DISTINCT
    r.RoomTypeId,
    r.RateDate,
    r.Rate,
    r.ServiceId AS PlanId,
    r.PlanName,
    r.PlanDescription,
    r.RoomType,
    r.RoomDescription
FROM MatchedRates r
ORDER BY r.RateDate
OPTION (MAXRECURSION 100);";
                int serviceId = 0;
                int.TryParse(inputDTO.ServiceId, out serviceId);
                var ratesPAram = new { @PlanId = inputDTO.CatId, @RoomTypeId = inputDTO.RoomType, @DateOfArrival = inputDTO.DateOfArrival, @ServiceId = serviceId };
                var enquiryRates = await _unitOfWork.GenOperations.GetTableData<RoomRatesForEnquiry>(ratesQuery, ratesPAram);
                if (enquiryRates != null && enquiryRates.Count > 0 && rooms != null)
                {
                    double dServiceId = 0;
                    double.TryParse(inputDTO.ServiceId, out dServiceId);
                    double distributedDiscount = (billingDTO.Discount == 0 || dServiceId == 0) ? 0 : ((billingDTO.Discount ?? 0) / dServiceId);

                    foreach (var room in rooms)
                    {
                        foreach (var item in enquiryRates)
                        {
                            var exitsrecinAudit = await _unitOfWork.AuditedRevenue.IsExists("Select * from [dbo].[AuditedRevenue] where RoomNumber=@RoomNumber and GroupId=@GroupId and Date=@Date and ChargesCategory=@ChargesCategory", new { @RoomNumber = room, @GroupId = inputDTO.GroupId, @Date = item.RateDate, @ChargesCategory = "RoomCharges" });

                            if (!exitsrecinAudit)
                            {
                                item.Rate = item.Rate - billingDTO.Discount;
                                AuditedRevenue auditedRevenue = new AuditedRevenue();
                                auditedRevenue.GroupId = inputDTO.GroupId;
                                auditedRevenue.RoomNumber = room;
                                auditedRevenue.Date = item.RateDate;
                                auditedRevenue.Charges = item.Rate;
                                auditedRevenue.Taxes = item.Rate * 0.05;
                                auditedRevenue.TotalDueAmount = item.Rate + (item.Rate * 0.05);
                                auditedRevenue.ChargesCategory = "RoomCharges";
                                auditedRevenue.IsActive = true;
                                auditedRevenue.CreatedDate = DateTime.Now;
                                auditedRevenue.CreatedBy = billingDTO.CreatedBy;

                                await _unitOfWork.AuditedRevenue.AddAsync(auditedRevenue);
                            }
                            else
                            {
                                var recordInAudit = await _unitOfWork.AuditedRevenue.GetEntityData<AuditedRevenue>("Select * from [dbo].[AuditedRevenue] where RoomNumber=@RoomNumber and GroupId=@GroupId and Date=@Date and ChargesCategory=@ChargesCategory", new { @RoomNumber = room, @GroupId = inputDTO.GroupId, @Date = item.RateDate, @ChargesCategory = "RoomCharges" });
                                if (recordInAudit != null)
                                {
                                    recordInAudit.Charges = recordInAudit.Charges + (item.Rate - billingDTO.Discount);
                                    recordInAudit.Taxes = (recordInAudit.Charges) * 0.05;
                                    recordInAudit.TotalDueAmount = recordInAudit.Charges + (recordInAudit.Charges * 0.05);
                                    recordInAudit.Balance = recordInAudit.TotalDueAmount - recordInAudit.Payments;
                                    recordInAudit.ModifiedBy = billingDTO.CreatedBy;
                                    recordInAudit.ModifiedDate = DateTime.Now;
                                    await _unitOfWork.AuditedRevenue.UpdateAsync(recordInAudit);
                                }
                            }
                        }
                    }

                    Billing billingUpdate = await _unitOfWork.Billing.GetEntityData<Billing>("Select * from billing where id=@Id", new { @Id = billingDTO.Id });
                    if (billingUpdate != null)
                    {
                        billingUpdate.PostedToAudit = 1;
                        await _unitOfWork.Billing.UpdateAsync(billingUpdate);
                    }
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    public async Task<bool> PostOtherChargesToAudit(BillingDTO billingDTO)
    {
        try
        {
            var inputDTO = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>("Select * from MembersDetails where Id=@Id", new { @Id = billingDTO.GuestId });

            if (inputDTO.GroupId != null)
            {
                string rQuery = "Select RNumber from RoomAllocation where GuestID in (Select Id from MembersDetails where GroupId=@GroupId) Group By RNumber";
                var rooms = await _unitOfWork.GenOperations.GetTableData<string>(rQuery, new { @GroupId = inputDTO.GroupId });

                string mdQuery = "Select * from MembersDetails where GroupId=@GroupId and PAxSno=1";
                var members = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>(mdQuery, new { @GroupId = inputDTO.GroupId });

                string ratesQuery1 = @"SELECT 
                            Distinct
                            r.RoomTypeId AS RoomTypeId,
                            r.[Date] AS RateDate,
                            r.Price AS Rate,
                            s.ID AS PlanId,
                            s.Service AS PlanName,
                            s.Description AS PlanDescription,
                            rt.RType RoomType,
	                        rt.Remarks RoomDescription
                        FROM Rates r
                        LEFT JOIN Services s ON r.PlanId = s.ID
                        LEFT JOIN RoomType rt ON r.RoomTypeId = rt.ID						
                        where r.PlanId=@PlanId 
						and r.RoomTypeId=@RoomTypeId
						and 
						r.[Date] >= 
						CAST(	@DateOfArrival	AS DATE) 
						AND r.[Date] < DATEADD(DAY, Cast( @ServiceId as int ), CAST( @DateOfArrival AS DATE))";
                string ratesQuery = @"WITH DateRange AS (
    SELECT @DateOfArrival AS RateDate
    UNION ALL
    SELECT DATEADD(DAY, 1, RateDate)
    FROM DateRange
    WHERE DATEADD(DAY, 1, RateDate) < DATEADD(DAY, @ServiceId, @DateOfArrival)
),
MatchedRates AS (
    SELECT 
        d.RateDate,
        r.RoomTypeId,
        r.PlanId,
        r.[Date] AS RateDateReal,
        r.Price AS Rate,
        s.ID AS ServiceId,
        s.Service AS PlanName,
        s.Description AS PlanDescription,
        rt.RType AS RoomType,
        rt.Remarks AS RoomDescription
    FROM DateRange d
    CROSS APPLY (
        SELECT TOP 1 *
        FROM Rates r
        WHERE 
            r.RoomTypeId = @RoomTypeId AND
            r.PlanId = @PlanId AND
            r.[Date] <= d.RateDate
        ORDER BY r.[Date] DESC
    ) r
    LEFT JOIN Services s ON r.PlanId = s.ID
    LEFT JOIN RoomType rt ON r.RoomTypeId = rt.ID
)
SELECT 
    DISTINCT
    r.RoomTypeId,
    r.RateDate,
    r.Rate,
    r.ServiceId AS PlanId,
    r.PlanName,
    r.PlanDescription,
    r.RoomType,
    r.RoomDescription
FROM MatchedRates r
ORDER BY r.RateDate
OPTION (MAXRECURSION 100);";
                int serviceId = 0;
                int.TryParse(inputDTO.ServiceId, out serviceId);
                var ratesPAram = new { @PlanId = inputDTO.CatId, @RoomTypeId = inputDTO.RoomType, @DateOfArrival = inputDTO.DateOfArrival, @ServiceId = serviceId };
                var enquiryRates = await _unitOfWork.GenOperations.GetTableData<RoomRatesForEnquiry>(ratesQuery, ratesPAram);
                if (enquiryRates != null && enquiryRates.Count > 0 && rooms != null)
                {
                    double dServiceId = 0;
                    double.TryParse(inputDTO.ServiceId, out dServiceId);
                    double distributedDiscount = (billingDTO.Discount == 0 || dServiceId == 0) ? 0 : ((billingDTO.Discount ?? 0) / dServiceId);

                    foreach (var room in rooms)
                    {
                        foreach (var item in enquiryRates)
                        {
                            var exitsrecinAudit = await _unitOfWork.AuditedRevenue.IsExists("Select * from [dbo].[AuditedRevenue] where RoomNumber=@RoomNumber and GroupId=@GroupId and Date=@Date and ChargesCategory=@ChargesCategory", new { @RoomNumber = room, @GroupId = inputDTO.GroupId, @Date = item.RateDate, @ChargesCategory = billingDTO.ServiceType });

                            if (!exitsrecinAudit)
                            {
                                item.Rate = billingDTO.Price - billingDTO.Discount;
                                AuditedRevenue auditedRevenue = new AuditedRevenue();
                                auditedRevenue.GroupId = inputDTO.GroupId;
                                auditedRevenue.RoomNumber = room;
                                auditedRevenue.Date = item.RateDate;
                                auditedRevenue.Charges = item.Rate;
                                auditedRevenue.Taxes = auditedRevenue.Charges * 0.05;
                                auditedRevenue.TotalDueAmount = auditedRevenue.Charges + (auditedRevenue.Charges * 0.05);
                                auditedRevenue.ChargesCategory = billingDTO.ServiceType;
                                auditedRevenue.IsActive = true;
                                auditedRevenue.CreatedDate = DateTime.Now;
                                auditedRevenue.CreatedBy = billingDTO.CreatedBy;

                                await _unitOfWork.AuditedRevenue.AddAsync(auditedRevenue);
                            }
                            else
                            {
                                var recordInAudit = await _unitOfWork.AuditedRevenue.GetEntityData<AuditedRevenue>("Select * from [dbo].[AuditedRevenue] where RoomNumber=@RoomNumber and GroupId=@GroupId and Date=@Date and ChargesCategory=@ChargesCategory", new { @RoomNumber = room, @GroupId = inputDTO.GroupId, @Date = item.RateDate, @ChargesCategory = billingDTO.ServiceType });
                                if (recordInAudit != null)
                                {
                                    recordInAudit.Charges = recordInAudit.Charges + (billingDTO.Price - billingDTO.Discount);
                                    recordInAudit.Taxes = (recordInAudit.Charges) * 0.05;
                                    recordInAudit.TotalDueAmount = recordInAudit.Charges + (recordInAudit.Charges * 0.05);
                                    recordInAudit.Balance = recordInAudit.TotalDueAmount - recordInAudit.Payments;
                                    recordInAudit.ModifiedBy = billingDTO.CreatedBy;
                                    recordInAudit.ModifiedDate = DateTime.Now;
                                    await _unitOfWork.AuditedRevenue.UpdateAsync(recordInAudit);
                                }
                            }
                        }
                    }

                    Billing billingUpdate = await _unitOfWork.Billing.GetEntityData<Billing>("Select * from billing where id=@Id", new { @Id = billingDTO.Id });
                    if (billingUpdate != null)
                    {
                        billingUpdate.PostedToAudit = 1;
                        await _unitOfWork.Billing.UpdateAsync(billingUpdate);
                    }
                    return true;

                }
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<bool> PostChargesToLedger(int GuestId)
    {
        try
        {
            GuestLedger guestLedger;
            string Query = @"SELECT Distinct GroupId,RNumber RoomNumber FROM MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId)and ra.RNumber IN(SELECT RNumber FROM RoomAllocation WHERE GuestID = @GuestId)";
            var sParam = new { @GuestId = GuestId };

            var GuestGroupRoomKey = await _unitOfWork.GenOperations.GetEntityData<GuestLedgerDTO>(Query, sParam);
            if (GuestGroupRoomKey != null)
            {
                double TotalAmount = await _unitOfWork.GenOperations.GetEntityData<double>(@"Select SUM(TotalAmount) from Billing b where GuestId in (SELECT md.Id FROM MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId)and ra.RNumber IN(SELECT RNumber FROM RoomAllocation WHERE GuestID = @GuestId)) and Confirmed=1 and IsActive=1 and b.ServiceType not in ('GrossAmount')", sParam);
                double TotalAmountWithTax = (TotalAmount) + (TotalAmount * 0.05);
                Query = "Select * from GuestLedger where GroupId=@GroupId and RoomNumber=@RoomNumber and IsActive=1";
                guestLedger = await _unitOfWork.GenOperations.GetEntityData<GuestLedger>(Query, new { @GroupId = GuestGroupRoomKey.GroupId, @RoomNumber = GuestGroupRoomKey.RoomNumber });

                if (guestLedger != null)
                {
                    guestLedger.TotalCharges = TotalAmountWithTax;
                    await _unitOfWork.GuestLedger.UpdateAsync(guestLedger);
                }
                else
                {
                    guestLedger = new GuestLedger();
                    guestLedger.GroupId = GuestGroupRoomKey.GroupId;
                    guestLedger.RoomNumber = GuestGroupRoomKey.RoomNumber;
                    guestLedger.TotalCharges = TotalAmountWithTax;
                    guestLedger.TotalPayment = 0;
                    guestLedger.IsActive = true;
                    await _unitOfWork.GuestLedger.AddAsync(guestLedger);
                }

            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }


    public async Task<bool> PostPaymentsToLedger(int GuestId)
    {
        try
        {
            GuestLedger guestLedger;
            string Query = @"SELECT Distinct GroupId,RNumber RoomNumber FROM MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId)and ra.RNumber IN(SELECT RNumber FROM RoomAllocation WHERE GuestID = @GuestId)";
            var sParam = new { @GuestId = GuestId };

            var GuestGroupRoomKey = await _unitOfWork.GenOperations.GetEntityData<GuestLedgerDTO>(Query, sParam);
            if (GuestGroupRoomKey != null)
            {
                double TotalAmount = await _unitOfWork.GenOperations.GetEntityData<double>(@"Select SUM(Amount) from Payment b where GuestId in (SELECT md.Id FROM MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId)and ra.RNumber IN(SELECT RNumber FROM RoomAllocation WHERE GuestID = @GuestId))", sParam);
                double TotalAmountWithTax = (TotalAmount) + (TotalAmount * 0.05);
                Query = "Select * from GuestLedger where GroupId=@GroupId and RoomNumber=@RoomNumber and IsActive=1";
                guestLedger = await _unitOfWork.GenOperations.GetEntityData<GuestLedger>(Query, new { @GroupId = GuestGroupRoomKey.GroupId, @RoomNumber = GuestGroupRoomKey.RoomNumber });

                if (guestLedger != null)
                {
                    guestLedger.TotalPayment = TotalAmount;
                    await _unitOfWork.GuestLedger.UpdateAsync(guestLedger);
                }
                else
                {
                    guestLedger = new GuestLedger();
                    guestLedger.GroupId = GuestGroupRoomKey.GroupId;
                    guestLedger.RoomNumber = GuestGroupRoomKey.RoomNumber;
                    guestLedger.TotalPayment = TotalAmount;
                    guestLedger.IsActive = true;
                    await _unitOfWork.GuestLedger.AddAsync(guestLedger);
                }

            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }

    public async Task<IActionResult> SavePaymentData(PaymentDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                inputDTO.Id = await _unitOfWork.Payment.AddAsync(_mapper.Map<Payment>(inputDTO));
                if (inputDTO.Id > 0)
                {
                    return Ok(inputDTO);
                }
            }
            return BadRequest("Some error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }
    public async Task<IActionResult> RemoveRecordFromBillingData(BillingDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var billingData = await _unitOfWork.GenOperations.GetEntityData<Billing>("Select * from Billing where Id=@Id", new { @Id = inputDTO.Id });
                if (billingData != null)
                {
                    billingData.IsActive = false;
                    bool updated = await _unitOfWork.Billing.UpdateAsync(billingData);
                    if (updated)
                    {
                        return Ok("Updated");
                    }
                }
            }
            return BadRequest("Some error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPaymentData(int GuestId)
    {
        try
        {
            string sQuery = "Select * from Payment where GuestId=@GuestId and IsActive=1";
            var sParam = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.GetTableData<PaymentDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPaymentDataWithAttr(int GuestId)
    {
        try
        {
            string sQuery = @"With GuestIds as(SELECT GuestID FROM RoomAllocation WHERE GuestID IN (SELECT Id FROM MembersDetails WHERE GroupId = (SELECT GroupId FROM MembersDetails WHERE Id = @GuestId))AND RNumber = (SELECT TOP 1 RNumber FROM RoomAllocation WHERE GuestID = @GuestId ORDER BY Id DESC ))
                            Select 
                            pm.PaymentMethodName
                            ,p.* 
                            from 
                            Payment p
                            Join PaymentMethod pm on p.PaymentMode=pm.Id
                            where GuestId in (Select GuestId from GuestIds) and p.IsActive=1";
            var sParam = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.GetTableData<PaymentWithAttr>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }
    public async Task<bool> IsAccountSettled(int GuestId)
    {
        try
        {
            string sQuery = @"With GuestIds as(SELECT GuestID FROM RoomAllocation WHERE GuestID IN (SELECT Id FROM MembersDetails WHERE GroupId = (SELECT GroupId FROM MembersDetails WHERE Id = @GuestId))AND RNumber = (SELECT TOP 1 RNumber FROM RoomAllocation WHERE GuestID = @GuestId ORDER BY Id DESC ))
                            Select 
            
                            p.* 
                            from 
                            Settlement p
                            
                            where GuestId in (Select GuestId from GuestIds) and IsActive=1";
            var sParam = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.IsExists(sQuery, sParam);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetBillingDataWithAttr(int GuestId)
    {
        try
        {
            //string sQuery = @" Select 
            //                (Case when b.ServiceType='Service' then tm.TaskName when b.ServiceType='PackageSystem' then s.Service when b.ServiceType='Package' then s.Service else '' end )ServiceName
            //                ,b.* 
            //                from Billing b
            //                Left Join Services s on b.ServiceId=s.ID
            //                Left Join TaskMaster tm on b.ServiceId=tm.ID
            //                where GuestId=@GuestId and b.IsActive=1";
            string sQuery = @" Select 
                                                        (Case when b.ServiceType='Service' then tm.TaskName when b.ServiceType='PackageSystem' then s.Service when b.ServiceType='Package' then s.Service else '' end )ServiceName
                                                        ,b.* 
                                                        from Billing b
                                                        Left Join Services s on b.ServiceId=s.ID
                                                        Left Join TaskMaster tm on b.ServiceId=tm.ID
                                                        where GuestId in (SELECT GuestID 
                                FROM RoomAllocation 
                                WHERE GuestID IN (
                                    SELECT Id 
                                    FROM MembersDetails 
                                    WHERE GroupId = (
                                        SELECT GroupId 
                                        FROM MembersDetails 
                                        WHERE Id = @GuestId
                                    )
                                )
                                AND RNumber = (
                                    SELECT TOP 1 RNumber 
                                    FROM RoomAllocation 
                                    WHERE GuestID = @GuestId 
                                    ORDER BY Id DESC  -- Replace AllocationDate with your actual date column
                                )) and b.IsActive=1";
            var sParam = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.GetTableData<BillingDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSettlementData(int GuestId)
    {
        try
        {
            string sQuery = @"Select * from Settlement where GuestId=@GuestId and IsActive=1";
            var sParam = new { @GuestId = GuestId };
            var res = await _unitOfWork.GenOperations.GetEntityData<SettlementDTO>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveSettlementInformation(SettlementDTO? inputDTO)
    {
        try
        {
            if (await ConfirmAllBillingCharges(1) == false)
            {
                return BadRequest("Error while confirming the charges. You can try confirming manually on billing page");
            }

            string sQueryBilling = "Select * from billing where IsActive=1 and ServiceType='GrossAmount' and GuestId=@GuestIdPaxSN1";
            //string sQueryPayment = @"Select * from Payment where IsActive=1 and GuestId IN (SELECT Id FROM MembersDetails WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId))";
            string sQueryPayment = @"Select * from Payment where IsActive=1 and GuestId IN (SELECT md.Id FROM MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId)and ra.RNumber IN(SELECT RNumber FROM RoomAllocation WHERE GuestID = @GuestId))";
            string sQuerySettlement = "Select * from Settlement where GuestId=@GuestIdPaxSN1 and IsActive=1";
            var sParam = new { @GuestId = inputDTO?.GuestId, @GuestIdPaxSN1 = inputDTO?.GuestIdPaxSN1 };

            var billingRes = await _unitOfWork.GenOperations.GetEntityData<BillingDTO>(sQueryBilling, sParam);
            var paymentRes = await _unitOfWork.GenOperations.GetTableData<PaymentDTO>(sQueryPayment, sParam);
            var settlementExists = await _unitOfWork.GenOperations.IsExists(sQuerySettlement, sParam);

            double totalAmount = billingRes.TotalAmount ?? 0;
            double totalPayment = paymentRes.Sum(x => x.Amount) ?? 0;

            double balance = totalAmount - totalPayment;

            if ((balance + inputDTO?.Refund + inputDTO?.CreditAmount) == 0)
            {
                if (settlementExists)
                {
                    return BadRequest("Settlement already exists for this guest");
                }
                else
                {
                    if (inputDTO != null)
                    {
                        inputDTO.InvoiceNumber = await GenerateNextInvoiceNumberAsync();
                        //return BadRequest("");
                    }

                    int insertedId = await _unitOfWork.Settlement.AddAsync(_mapper.Map<Settlement>(inputDTO));
                    if (insertedId > 0)
                    {
                        CreditDebitNoteAccountDTO creditDebitNoteAccountDTO = new CreditDebitNoteAccountDTO();

                        creditDebitNoteAccountDTO.Code = inputDTO?.NoteNumber;
                        creditDebitNoteAccountDTO.Amount = inputDTO?.CreditAmount;
                        creditDebitNoteAccountDTO.CodeValidity = inputDTO?.ValidTill;
                        creditDebitNoteAccountDTO.CreatedDate = DateTime.Now;
                        creditDebitNoteAccountDTO.CreatedBy = inputDTO.CreatedBy;
                        creditDebitNoteAccountDTO.IsActive = true;
                        creditDebitNoteAccountDTO.TransactionType = "Credit";

                        await _unitOfWork.CreditDebitNoteAccount.AddAsync(_mapper.Map<CreditDebitNoteAccount>(creditDebitNoteAccountDTO));

                        await PostBulkDataToAudit(1);



                        return Ok("Settlement saved successfully");
                    }
                    else
                    {
                        return BadRequest("Error occurred while settling account");
                    }

                }
            }
            else
            {
                return BadRequest("Settlement amount is not equal to zero");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }

    public async Task<bool> ConfirmAllBillingCharges(int GuestId)
    {
        try
        {
            string query = @"Update b set Confirmed=1 from Billing b where GuestId in (SELECT md.Id FROM MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId)and ra.RNumber IN(SELECT RNumber FROM RoomAllocation WHERE GuestID = @GuestId)) and Confirmed=1 and IsActive=1 and b.ServiceType not in ('GrossAmount')";

            var updated = await _unitOfWork.GenOperations.ExecuteQuery(query, new { @GuestId = GuestId });

            await PostChargesToLedger(GuestId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }

    public async Task<bool> PostBulkDataToAudit(int GuestId)
    {
        try
        {
            //int GuestId = 0;
            //if (inputDTO != null && inputDTO.BillingDTOs != null && inputDTO.BillingDTOs.Count > 0)
            //{
            //    GuestId = inputDTO?.BillingDTOs?.FirstOrDefault()?.GuestId ?? 0;
            //}

            string memberDetailsQuery = @"Select * from Billing s where GuestId in (
                        SELECT md.Id
                        --, md.CustomerName, md.GroupId, ra.Rnumber
                        FROM MembersDetails md
                        INNER JOIN RoomAllocation ra ON md.Id = ra.GuestID AND ra.IsActive = 1 and md.Status=1
                        WHERE md.GroupId = (
                            SELECT md2.GroupId
                            FROM MembersDetails md2
                            WHERE md2.Id = @GuestId and md2.Status=1
                        )
                        AND ra.Rnumber = (
                            SELECT ra2.Rnumber
                            FROM RoomAllocation ra2
                            WHERE ra2.GuestID = @GuestId AND ra2.IsActive = 1
                        )
                        ) and s.isactive=1 and ISNULL(PostedToAudit,0)!=1 and s.ServiceType not in ('GrossAmount');";
            var billings = await _unitOfWork.GenOperations.GetTableData<BillingDTO>(memberDetailsQuery, new { @GuestId = GuestId });

            if (billings != null)
            {
                foreach (var item in billings)
                {
                    if (item.ServiceType == "RoomCharges")
                    {
                        await PostRoomChargesToAudit(item);
                    }
                    else
                    {
                        await PostOtherChargesToAudit(item);
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }

    public async Task<string> GenerateNextInvoiceNumberAsync()
    {
        string prefix = "NWPL";
        string yearMonth = DateTime.Now.ToString("yyyyMM");

        string invoiceQuery = @"SELECT TOP 1 InvoiceNumber 
                            FROM Settlement 
                            WHERE InvoiceNumber LIKE 'NWPL%' 
                            ORDER BY InvoiceNumber DESC";

        var lastInvoice = await _unitOfWork.GenOperations.GetEntityData<string>(invoiceQuery, null);

        int newSerial = 1;
        if (!string.IsNullOrEmpty(lastInvoice) && lastInvoice.Length >= 15)
        {
            string lastSerialStr = lastInvoice.Substring(10, 5);
            if (int.TryParse(lastSerialStr, out int lastSerial))
            {
                newSerial = lastSerial + 1;
            }
        }

        string newInvoiceNumber;
        bool exists;

        do
        {
            newInvoiceNumber = $"{prefix}{yearMonth}{newSerial.ToString("D5")}";

            // Check if invoice number already exists
            string checkQuery = "SELECT * FROM Settlement WHERE InvoiceNumber = @InvoiceNumber";
            exists = await _unitOfWork.GenOperations.IsExists(checkQuery, new { InvoiceNumber = newInvoiceNumber });

            if (exists)
            {
                newSerial++;
            }

        } while (exists);

        return newInvoiceNumber;
    }



    public async Task<IActionResult> GetAllGuestsInRoom(int GuestId)
    {
        try
        {
            string sQuery = @"Select 
                            md.Id
                            ,rt.RType RoomTypeName 
                            ,RoomType
                            ,gm.Gender as GenderName
                            ,[FName]
                            ,[MName]
                            ,[LName]
                            ,CustomerName
                            ,[PhNo]
                            ,countries.iso3 [CountryCode]
                            ,[MobileNo]
                            ,[EmergencyNo]
                            ,[DOB]
                            ,(Case when [MarStatus]='S' then 'Single' when [MarStatus]='M' then 'Married' when [MarStatus]='Se' then 'Separated' when [MarStatus]='D' then 'Divorce' when [MarStatus]='O' then 'Others'  else '' end)   [MarStatus]
                                       ,md.[Gender]
                                       ,[PhysicianName]
                                       ,[PhysicianNumber]
                                       ,[ReferredBy]
                                       ,[ServiceId]
                                       ,[LegalName]
                                       ,[Address1]
                                       ,[Address2]
                                       ,[Pincode]
                                       ,[Country]
                                       ,[Email]
                                       ,[IDProof]
                                       ,[IsMonthlynewsletter]
                                       ,[AboutUs]
                                       ,[RelativeName]
                                       ,[Relations]
                                       ,[RelativeNumber]
                                       ,[ReferContact]
                                       ,[UniqueNo]
                                       ,md.[Status]
                                       ,[IsApproved]
                                       ,md.[Remarks]
                                       ,[UserId]
                                       ,[ApprovedBy]
                                       ,[CreationDate]
                                       ,[AprovedDate]
                                       ,[Photo]
                                       ,[BloodGroup]
                                       ,[Occupation]
                                       ,[PassportNo]
									   ,PassportIssueDate
									   ,PassportExpiryDate
                                       ,[VisaDetails]
									   ,PhotoShared
									   ,VisaIssueDate
									   ,VisaExpiryDate
                                       ,IDProof
                                       ,IDProofIssueDate
                                       ,IdProofExpiryDate
                                       ,[PolicyHolder]
                                       ,[InsuranceCompany]
                                       ,[PolicyNo]
                                       ,[CoveredIndia]
                                       ,[Staydays]
                                       ,[RoomType]
                                       ,[HealthIssues]
                                       ,[CatID]
                                       ,[IsCRM]
                                       ,[Nights]
                                       ,countries.nationality [Nationality]
                                       ,NationalityId
                                       ,[GuarenteeCode]
                                       ,[PaymentStatus]
                                       ,[HoldTillDate]
                                       ,[PaymentDate]
                                       ,[LeadSource]
                                       ,[ChannelCode]
                                       ,[AdditionalNights]
                                       ,(Case when ra.CheckInDate is not null then ra.CheckInDate else md.[DateOfArrival] end)[DateOfArrival]
                                       ,(Case when ra.CheckOutDate is not null then ra.CheckOutDate else md.[DateOfDepartment] end)[DateOfDepartment]
                                       ,[PAX]
                                       ,[NoOfRooms]
                                       ,[PickUpDrop]
                                       ,[PickUpType]
                                       ,[CarType]
                                       ,[FlightArrivalDateAndDateTime]
                                       ,[FlightDepartureDateAndDateTime]
                                       ,[PickUpLoaction]
                                       ,[Age]
                                       ,[GroupId]
                                       ,[PAXSno]
                                       ,[PaxCompleted]
                                       ,[RegistrationNumber]
                                       ,[UHID]
                                       ,isnull((Select AVG(Price) from rates r where r.[Date] between md.DateOfArrival and md.DateOfDepartment and  roomtypeid=md.RoomType and PlanId=md.CatID),(SELECT TOP 1 r.Price FROM Rates r WHERE r.Date <= ISNULL(ra.CheckInDate, md.DateOfArrival) AND r.RoomTypeId = md.RoomType AND r.PlanId = md.CatID ORDER BY r.Date DESC ))RoomPrice
                            ,c.name City
                            ,s.name State  
                            ,countries.name  Country
                            ,ra.RNumber RoomNumber
                            ,ss.[Service]
                            --,isnull(md.ServiceId,0) + isnull(md.AdditionalNights,0) as NoOfNights
                            ,DATEDIFF(DAY,  isnull(ra.CheckInDate,md.DateOfArrival) ,  isnull(ra.CheckOutDate,md.DateOfDepartment) ) NoOfNights
                            ,ss.Price PackagePrice
                            from MembersDetails md 
                            Left Join GenderMaster gm on md.Gender=gm.Id 
                            Left Join cities c on md.CityId=c.ID 
                            Left Join states s on md.StateId=s.ID
                            Left Join countries countries on countries.id=md.CountryId
                            Left join RoomType rt on  md.RoomType = rt.ID  
                            Left Join RoomAllocation ra on ra.GuestID=md.Id
                            Left Join Services ss on md.CatID=ss.ID
                            where md.Status=1 and ra.IsActive=1 and md.Id
							in
							(SELECT GuestID 
                                FROM RoomAllocation 
                                WHERE GuestID IN (
                                    SELECT Id 
                                    FROM MembersDetails 
                                    WHERE GroupId = (
                                        SELECT GroupId 
                                        FROM MembersDetails 
                                        WHERE Id = @GuestId
                                    )
                                )
                                AND RNumber = (
                                    SELECT TOP 1 RNumber 
                                    FROM RoomAllocation 
                                    WHERE GuestID = @GuestId 
                                    ORDER BY Id DESC  -- Replace AllocationDate with your actual date column
                                ))";

            var sParam = new { @GuestId = GuestId };

            var memberDetails = await _unitOfWork.GenOperations.GetTableData<MemberDetailsWithChild>(sQuery, sParam);

            return Ok(memberDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }

    public async Task<IActionResult> GetEnquiryList(int ScheduleType)
    {
        try
        {
            string sQuery = @"Select 
                            gr.ID
                            ,FName
                            ,Mname
                            ,Lname
                            ,MobileNo
                            ,EmailId
                            ,CatId
                            ,mst.Service RatePlan
                            ,DateOfArrival
                            ,DateOfDepartment
                            ,Pax
                            ,RoomType
                            ,rt.RType RoomTypeName
                            ,gr.ServiceId
                            ,NoOfRooms
                            ,gr.Remarks
                            ,Pax
                            ,NoOfRooms
                            ,gr.SaleSource
                            ,gr.LeadSource 
                            ,gr.BrandAwn 
                            ,sou.Name  SaleSourceName
                            ,ls.LeadSource LeadSourceName
                            ,ba.Awareness BrandAwarenessName
                            from [dbo].[GuestReservation] gr
                            Left Join Services mst on mst.ID=gr.CatID
                            Left Join RoomType rt on gr.RoomType=rt.ID
                            Left Join Sources sou on sou.ID=gr.SaleSource
                            Left Join LeadSource ls on ls.Id=gr.LeadSource
                            Left Join BrandAwareness ba on ba.ID=gr.BrandAwn
                            where gr.status=1 and ScheduleType=@ScheduleType
                            order by gr.id desc";
            var sParam = new { @ScheduleType = ScheduleType };
            var res = await _unitOfWork.GenOperations.GetTableData<GuestReservationWithAttr>(sQuery, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }

    public async Task<IActionResult> UpdateGuestReservationDetails(GuestsListViewModel dataVM)
    {
        try
        {
            bool error = false;
            bool ArrivalDateChangeNotAllowedFlag = false;
            _unitOfWork.BeginTransaction();
            if (dataVM != null && dataVM.MemberDetail != null)
            {
                var memberDetailsList = await _unitOfWork.GenOperations.GetTableData<MemberDetailsWithChild>(@"SELECT md.*,ra.RNumber RoomNo FROM MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID WHERE GroupId=(SELECT GroupId FROM MembersDetails WHERE Id = @GuestId)and ra.RNumber IN(SELECT RNumber FROM RoomAllocation WHERE GuestID = @GuestId)", new { @GuestId = dataVM.MemberDetail.Id });

                var memberDetail = await _unitOfWork.GenOperations.GetEntityData<MemberDetailsWithChild>(@"Select *,ra.RNumber RoomNo,ra.CheckInDate from MembersDetails md Left Join RoomAllocation ra on md.Id=ra.GuestID where md.id=@GuestId", new { @GuestId = dataVM.MemberDetail.Id });

                memberDetail.CatId = dataVM.MemberDetail.CatId;
                memberDetail.ServiceId = dataVM.MemberDetail.ServiceId;

                if (!(memberDetail.CheckInDate != null) || memberDetail.DateOfArrival >= DateTime.Today)
                {
                    memberDetail.DateOfArrival = dataVM.MemberDetail.DateOfArrival;
                }
                else
                {
                    //memberDetail.DateOfArrival = dataVM.MemberDetail.DateOfArrival;
                    if (memberDetail.DateOfArrival != dataVM.MemberDetail.DateOfArrival)
                        ArrivalDateChangeNotAllowedFlag = true;
                }
                memberDetail.DateOfDepartment = dataVM.MemberDetail.DateOfDepartment;
                memberDetail.RoomType = dataVM.MemberDetail.RoomType;

                if (memberDetailsList != null)
                {
                    foreach (var item in memberDetailsList)
                    {
                        MembersDetails md = await _unitOfWork.GenOperations.GetEntityData<MembersDetails>(@"Select * from MembersDetails where Id=@Id", new { @Id = item.Id });
                        md.CatId = dataVM.MemberDetail.CatId;
                        md.ServiceId = dataVM.MemberDetail.ServiceId;
                        md.DateOfArrival = dataVM.MemberDetail.DateOfArrival;
                        md.DateOfDepartment = dataVM.MemberDetail.DateOfDepartment;
                        md.RoomType = dataVM.MemberDetail.RoomType;
                        bool mdDetailsUpdated = await _unitOfWork.MembersDetails.UpdateAsync(md);
                        if (!mdDetailsUpdated)
                        {
                            error = true;
                        }

                        RoomAllocation ra = await _unitOfWork.GenOperations.GetEntityData<RoomAllocation>(@"Select * from RoomAllocation where GuestId=@GuestId", new { @GuestId = item.Id });
                        ra.Fd = dataVM.MemberDetail.DateOfArrival;
                        ra.Td = dataVM.MemberDetail.DateOfDepartment;
                        ra.Rtype = dataVM.MemberDetail.RoomType;
                        bool raUpdated = await _unitOfWork.RoomAllocation.UpdateAsync(ra);
                        if (raUpdated == false)
                        {
                            error = true;
                        }
                    }

                }


                var isRoomAvailable = await IsRoomsAvailable(memberDetail);
                if (isRoomAvailable is not OkObjectResult)
                {
                    error = false;
                }



                bool roomChargesUpdated = await UpdatePostedRoomCharges(memberDetail);
                if (roomChargesUpdated == false)
                {
                    error = true;
                }
                //_unitOfWork.Rollback();
                if (error == false)
                {
                    _unitOfWork.Commit();
                    if (ArrivalDateChangeNotAllowedFlag)
                    {
                        return Ok("Arrival date change is not allowed for this guest.");
                    }
                    return Ok("Updated");
                }
                else
                {
                    _unitOfWork.Rollback();
                    return BadRequest("Data saved with errors.");
                }

            }
            else
            {
                return BadRequest("The guest details are invalid;");
            }
            return BadRequest("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePaymentData)}");
            throw;
        }
    }

    public async Task<bool> UpdatePostedRoomCharges(MemberDetailsWithChild inputDTO)
    {
        try
        {
            if (inputDTO.GroupId != null)
            {

                string rQuery = "Select RNumber from RoomAllocation where GuestID in (Select Id from MembersDetails where GroupId=@GroupId) Group By RNumber";
                var rooms = await _unitOfWork.GenOperations.GetTableData<string>(rQuery, new { @GroupId = inputDTO.GroupId });


                //Delete all AuditRevenueEntries

                if (!(inputDTO.CheckInDate != null) || inputDTO.DateOfArrival >= DateTime.Today)
                {

                }
                else
                {
                    inputDTO.DateOfArrival = DateTime.Now;
                }

                await _unitOfWork.AuditedRevenue.RunSQLCommand("Update AuditedRevenue set IsActive=0 where GroupId=@GroupId and RoomNumber=@Rooms and Date >= @Date", new { @GroupId = inputDTO.GroupId, @Rooms = rooms.FirstOrDefault(), @Date = inputDTO.DateOfArrival });

                //End



                string mdQuery = "Select * from MembersDetails where GroupId=@GroupId and PAxSno=1";
                var members = await _unitOfWork.GenOperations.GetEntityData<MembersDetailsDTO>(mdQuery, new { @GroupId = inputDTO.GroupId });

                string ratesQuery = @"SELECT 
                            Distinct
                            r.RoomTypeId AS RoomTypeId,
                            r.[Date] AS RateDate,
                            r.Price AS Rate,
                            s.ID AS PlanId,
                            s.Service AS PlanName,
                            s.Description AS PlanDescription,
                            rt.RType RoomType,
	                        rt.Remarks RoomDescription
                        FROM Rates r
                        LEFT JOIN Services s ON r.PlanId = s.ID
                        LEFT JOIN RoomType rt ON r.RoomTypeId = rt.ID						
                        where r.PlanId=@PlanId 
						and r.RoomTypeId=@RoomTypeId
						and 
						r.[Date] >= 
						CAST(	@DateOfArrival	AS DATE) 
						AND r.[Date] < DATEADD(DAY, Cast( @ServiceId as int ), CAST( @DateOfArrival AS DATE))";
                var ratesPAram = new { @PlanId = inputDTO.CatId, @RoomTypeId = inputDTO.RoomType, @DateOfArrival = inputDTO.DateOfArrival, @ServiceId = inputDTO.ServiceId };
                var enquiryRates = await _unitOfWork.GenOperations.GetTableData<RoomRatesForEnquiry>(ratesQuery, ratesPAram);
                if (enquiryRates != null && enquiryRates.Count > 0 && rooms != null)
                {
                    foreach (var room in rooms)
                    {
                        foreach (var item in enquiryRates)
                        {
                            var exitsrecinAudit = await _unitOfWork.AuditedRevenue.IsExists("Select * from [dbo].[AuditedRevenue] where RoomNumber=@RoomNumber and GroupId=@GroupId and Date=@Date and ChargesCategory=@ChargesCategory", new { @RoomNumber = room, @GroupId = inputDTO.GroupId, @Date = item.RateDate, @ChargesCategory = "RoomCharges" });

                            if (!exitsrecinAudit)
                            {
                                AuditedRevenue auditedRevenue = new AuditedRevenue();
                                auditedRevenue.GroupId = inputDTO.GroupId;
                                auditedRevenue.RoomNumber = room;
                                auditedRevenue.Date = item.RateDate;
                                auditedRevenue.Charges = item.Rate;
                                auditedRevenue.Taxes = item.Rate * 0.15;
                                auditedRevenue.TotalDueAmount = item.Rate + (item.Rate * 0.15);
                                auditedRevenue.ChargesCategory = "RoomCharges";
                                auditedRevenue.IsActive = true;
                                auditedRevenue.CreatedDate = DateTime.Now;
                                auditedRevenue.CreatedBy = inputDTO.LoggedInUser;

                                await _unitOfWork.AuditedRevenue.AddAsync(auditedRevenue);
                            }


                        }
                    }
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(FetchDepartmentDate)}");
            throw;
        }
    }
    private async Task<string?> GetAvailableRoomNumber(int? roomTypeId, DateTime? checkIn, DateTime? checkOut)
    {
        if (roomTypeId == null)
        {
            return null;
        }

        string query = @"SELECT TOP 1 r.RNumber AS Rnumber
                          FROM Rooms r
                            WHERE r.RtypeId = @RoomTypeId AND r.Status = 1 AND r.RNumber NOT IN (
                              SELECT ra.RNumber FROM RoomAllocation ra
                              WHERE ra.Rtype = @RoomTypeId AND ra.IsActive = 1 AND ISNULL(ra.Cancelled, 0) = 0 AND ((@FD BETWEEN ra.FD AND ra.TD) OR (@TD BETWEEN ra.FD AND ra.TD) OR (ra.FD BETWEEN @FD AND @TD) OR (ra.TD BETWEEN @FD AND @TD))
                          )";

        var parameters = new { RoomTypeId = roomTypeId, FD = checkIn, TD = checkOut };
        var room = await _unitOfWork.GenOperations.GetEntityData<RoomAllocationDTO>(query, parameters);
        return room?.Rnumber;
    }

}
