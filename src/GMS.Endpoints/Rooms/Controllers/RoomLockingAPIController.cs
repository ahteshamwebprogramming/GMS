using AutoMapper;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Rooms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace GMS.Endpoints.Rooms;

[Route("api/[controller]")]
[ApiController]
public class RoomLockingAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomLockingAPIController> _logger;
    private readonly IMapper _mapper;
    public RoomLockingAPIController(IUnitOfWork unitOfWork, ILogger<RoomLockingAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetRooms()
    {
        try
        {
            //string query = "Select * from GMSFinalGuest order by id desc";
            string query = @"Select * from Rooms where Status=1";
            var res = await _unitOfWork.Rooms.GetTableData<RoomsDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomById(int Id)
    {
        try
        {
            string query = @"Select * from Rooms where ID=@Id";
            var param = new { @Id = Id };
            var res = await _unitOfWork.Rooms.GetEntityData<RoomsDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomAmenetiesByRoomNumber(string RoomNumber)
    {
        try
        {
            string query = @"Select * from RoomAmeneties where RoomNumber=@RoomNumber";
            var param = new { @RoomNumber = RoomNumber };
            var res = await _unitOfWork.RoomAmeneties.GetTableData<RoomAmenetiesDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomAmenetiesWithChildByRoomNumber(string RoomNumber)
    {
        try
        {
            string query = @"Select ra.*,a.AmenityName AmenityName from RoomAmeneties ra
                            Join Amenities a on ra.AmenityId=a.Id
                            where ra.RoomNumber=@RoomNumber";
            var param = new { @RoomNumber = RoomNumber };
            var res = await _unitOfWork.RoomAmeneties.GetTableData<RoomAmenetiesWithChild>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomImagesByRoomNumber(int RoomId)
    {
        try
        {
            string query = @"Select * from RoomsPictures where RoomId=@RoomId";
            var param = new { @RoomId = RoomId };
            var res = await _unitOfWork.RoomsPictures.GetTableData<RoomsPicturesDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }

    public async Task<IActionResult> GetRoomTypes()
    {
        try
        {
            //string query = "Select * from GMSFinalGuest order by id desc";
            string query = @"Select * from roomtype where Status=1";
            var res = await _unitOfWork.RoomType.GetTableData<RoomTypeDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomAmeneties()
    {
        try
        {

            string query = @"Select * from Amenities where AmenityCategoryId=1 and IsActive=1";
            var res = await _unitOfWork.Amenities.GetTableData<AmenitiesDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> AddAmeneties(List<RoomAmenetiesDTO> inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var dQuery = "delete from RoomAmeneties where RoomNumber=@RoomNumber";
                var dParam = new { @RoomNumber = inputDTO?.FirstOrDefault()?.RoomNumber };
                await _unitOfWork.RoomAmeneties.RunSQLCommand(dQuery, dParam);

                var res = await _unitOfWork.RoomAmeneties.AddAsync(_mapper.Map<List<RoomAmeneties>>(inputDTO));
                if (res)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomsWithLockAndHoldStatus()
    {
        try
        {
            //string query = "Select * from GMSFinalGuest order by id desc";
            string query = @"declare @CurrentDate date = getdate();
                                Select 
                                * 
                                ,(Case when  
	                                (SELECT count(1) FROM RoomLock rl WHERE  CAST(FD AS DATE) <= @CurrentDate AND CAST(ED AS DATE) >= @CurrentDate AND Status = 1 and rl.Rooms=r.RNumber and rl.[Type]='Lock') > 0 then 'Locked'
		                                when 
	                                (SELECT count(1) FROM RoomLock rl WHERE  CAST(FD AS DATE) <= @CurrentDate AND CAST(ED AS DATE) >= @CurrentDate AND Status = 1 and rl.Rooms=r.RNumber and rl.[Type]='Hold') > 0 then 'Hold'
	                                else 'Available' end
                                ) [RoomStatus]

                                from Rooms r

                                where Status=1";
            var res = await _unitOfWork.Rooms.GetTableData<RoomsWithStatusDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomLockingList()
    {
        try
        {
            string query = @"Select * from RoomLock where status=1 order by id desc";
            var res = await _unitOfWork.RoomLock.GetTableData<RoomLockDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> UnHold(RoomLockDAO? inputDTO)
    {
        try
        {
            string query = @"declare @CurrentDate date = getdate();
                            Declare @Id int,@FD nvarchar(20);
                            SELECT @Id=ID,@FD=FD FROM RoomLock rl WHERE  CAST(FD AS DATE) <= @CurrentDate AND CAST(ED AS DATE) >= @CurrentDate AND Status = 1 and rl.Rooms=@RoomNo and rl.[Type]=@LockType
                            SELECT * FROM RoomLock rl WHERE  CAST(FD AS DATE) <= @CurrentDate AND CAST(ED AS DATE) >= @CurrentDate AND Status = 1 and rl.Rooms=@RoomNo and rl.[Type]=@LockType
                            if(CAST(@FD AS DATE) >= @CurrentDate)
                            begin
                            Update RoomLock set Status=0 where ID=@Id
                            end
                            else
                            begin
                            Update RoomLock set ED=CAST(@CurrentDate AS DATE) where ID=@Id
                            end";
            var sParam = new { @RoomNo = inputDTO?.RoomNo, @LockType = inputDTO?.RoomLockHold };
            var res = await _unitOfWork.RoomLock.ExecuteQuery(query, sParam);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveRoomLockingDetails(RoomLockDTO? inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.Id == 0)
                {
                    string eQuery = @"DECLARE @CheckFD DATE = Convert(Date,@FromDate); -- New FromDate to check
                                            DECLARE @CheckED DATE = Convert(Date,@EndDate); -- New EndDate to check

                                            SELECT *
                                            FROM RoomLock
                                            WHERE 
                                                -- Convert FD and ED to DATE for comparison
                                                (CAST(FD AS DATE) <= @CheckED AND CAST(ED AS DATE) >= @CheckFD) and Rooms = @Rooms;";
                    var eParam = new { @FromDate = inputDTO.Fd, @EndDate = inputDTO.Ed, @Rooms = inputDTO.Rooms };

                    var exists = await _unitOfWork.RoomLock.IsExists(eQuery, eParam);

                    if (exists)
                    {
                        return BadRequest("This Rooms is already locked for this daterange and part of this date range");
                    }
                    else
                    {
                        inputDTO.Id = await _unitOfWork.RoomLock.AddAsync(_mapper.Map<RoomLock>(inputDTO));
                        if (inputDTO.Id == 0)
                        {
                            return BadRequest("Unable to lock the room right now. Please try again later");
                        }
                        else
                        {
                            return Ok(inputDTO);
                        }
                    }
                }
                else
                {
                    string eQuery = @"DECLARE @CheckFD DATE = Convert(Date,@FromDate); -- New FromDate to check
                                            DECLARE @CheckED DATE = Convert(Date,@EndDate); -- New EndDate to check

                                            SELECT *
                                            FROM RoomLock
                                            WHERE 
                                                -- Convert FD and ED to DATE for comparison
                                                (CAST(FD AS DATE) <= @CheckED AND CAST(ED AS DATE) >= @CheckFD) and Rooms = @Rooms and Id!=@Id;";
                    var eParam = new { @FromDate = inputDTO.Fd, @EndDate = inputDTO.Ed, @Rooms = inputDTO.Rooms, @Id = inputDTO.Id };

                    var exists = await _unitOfWork.RoomLock.IsExists(eQuery, eParam);

                    if (exists)
                    {
                        return BadRequest("This Rooms is already locked for this daterange and part of this date range");
                    }
                    else
                    {
                        RoomLock? roomLock = await _unitOfWork.RoomLock.GetEntityData<RoomLock>("Select * from RoomLock where Id=@Id", new { @Id = inputDTO.Id });
                        if (roomLock != null)
                        {
                            roomLock.Fd = inputDTO.Fd;
                            roomLock.Ed = inputDTO.Ed;
                            roomLock.Rooms = inputDTO.Rooms;
                            roomLock.Remarks = inputDTO.Remarks;
                            var updated = await _unitOfWork.RoomLock.UpdateAsync(roomLock);
                            if (updated)
                            {
                                return Ok(roomLock);
                            }
                            else
                            {
                                return BadRequest("Unable to lock the room right now. Please try again later");
                            }
                        }
                        else
                        {
                            return BadRequest("Unable to find the record on this room number");
                        }
                    }
                }
            }
            else
            {
                return BadRequest("Invalid Data");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveRoom(RoomsDTO? inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                string RoomTypeQuery = "Select * from RoomType where Id=@RTypeId";
                var RoomTypeParam = new { @RTypeId = inputDTO.RtypeId };
                var RoomTypeEntity = await _unitOfWork.RoomType.GetEntityData<RoomTypeDTO>(RoomTypeQuery, RoomTypeParam);
                if (RoomTypeEntity != null)
                {
                    inputDTO.Rtype = RoomTypeEntity.Rtype;
                }
                if (inputDTO.Id > 0)
                {
                    string eQuery = "Select * from Rooms where RNumber=@RNumber and Id!=@Id";
                    var eParam = new { @RNumber = inputDTO.Rnumber, @Id = inputDTO.Id };
                    var roomNumberExists = await _unitOfWork.GenOperations.IsExists(eQuery, eParam);
                    if (roomNumberExists)
                    {
                        return BadRequest("This Room Number already exists");
                    }
                    else
                    {

                        string sQuery = "Select * from Rooms where Id=@Id";
                        var sParam = new { @Id = inputDTO.Id };
                        var roomRecord = await _unitOfWork.Rooms.GetEntityData<RoomsDTO>(sQuery, sParam);
                        if (roomRecord != null)
                        {
                            inputDTO.Status = roomRecord.Status;
                            inputDTO.IsChecked = roomRecord.IsChecked;
                            inputDTO.Img1 = roomRecord.Img1;
                            inputDTO.Img2 = roomRecord.Img2;
                            inputDTO.Img3 = roomRecord.Img3;
                            inputDTO.Img4 = roomRecord.Img4;
                            inputDTO.Img5 = roomRecord.Img5;
                            inputDTO.DocName = roomRecord.DocName;
                            var roomUpdated = await _unitOfWork.Rooms.UpdateAsync(_mapper.Map<GMS.Core.Entities.Rooms>(inputDTO));
                            if (roomUpdated)
                            {
                                return Ok(inputDTO);
                            }
                            else
                            {
                                return BadRequest("Unable to add room right now. Try again later");
                            }
                        }
                    }
                }
                else
                {
                    string eQuery = "Select * from Rooms where RNumber=@RNumber";
                    var eParam = new { @RNumber = inputDTO.Rnumber };
                    var roomNumberExists = await _unitOfWork.GenOperations.IsExists(eQuery, eParam);
                    if (roomNumberExists)
                    {
                        return BadRequest("This Room Number already exists");
                    }
                    else
                    {
                        inputDTO.Status = 1;
                        inputDTO.Id = await _unitOfWork.Rooms.AddAsync(_mapper.Map<GMS.Core.Entities.Rooms>(inputDTO));
                        if (inputDTO.Id > 0)
                        {
                            return Ok(inputDTO);
                        }
                        else
                        {
                            return BadRequest("Unable to add room right now. Try again later");
                        }
                    }
                }
            }
            else
            {
                return BadRequest("Invalid Data");
            }
            return BadRequest("Some error has occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }

    public async Task<IActionResult> CheckAndAddPictureToDatabase(RoomsPicturesDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                string eQuery = "Select * from RoomsPictures where RoomId=@RoomId and AttachmentName=@AttachmentName and IsActive=1";
                var eParam = new { @RoomId = inputDTO.RoomId, @AttachmentName = inputDTO.AttachmentName };
                var exists = await _unitOfWork.RoomsPictures.IsExists(eQuery, eParam);
                if (exists)
                {
                    inputDTO.AttachmentName = inputDTO.AttachmentName + " (Copy)";
                }
                inputDTO.Id = await _unitOfWork.RoomsPictures.AddAsync(_mapper.Map<RoomsPictures>(inputDTO));
                if (inputDTO.Id > 0)
                {
                    return Ok(inputDTO);
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            else
            {
                return BadRequest("Invalid Data");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetRooms)}");
            throw;
        }
    }
}
