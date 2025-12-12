using AutoMapper;
using FluentAssertions;
using GMS.Core.Entities;
using GMS.Core.Repository;
using GMS.Endpoints.Guests;
using GMS.Infrastructure.Models.Guests;
using GMS.Infrastructure.Models.Rooms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GMS.Endpoints.Tests;

public class GuestsAPIControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMembersDetailsRepository> _membersDetailsRepositoryMock = new();
    private readonly Mock<IGenOperationsRepository> _genOperationsRepositoryMock = new();
    private readonly Mock<IBillingRepository> _billingRepositoryMock = new();
    private readonly Mock<IRoomAllocationRepository> _roomAllocationRepositoryMock = new();
    private readonly IMapper _mapper;
    private readonly GuestsAPIController _controller;

    public GuestsAPIControllerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<MembersDetailsDTO, MembersDetails>();
        });
        _mapper = mapperConfig.CreateMapper();

        _unitOfWorkMock.SetupGet(u => u.MembersDetails).Returns(_membersDetailsRepositoryMock.Object);
        _unitOfWorkMock.SetupGet(u => u.GenOperations).Returns(_genOperationsRepositoryMock.Object);
        _unitOfWorkMock.SetupGet(u => u.Billing).Returns(_billingRepositoryMock.Object);
        _unitOfWorkMock.SetupGet(u => u.RoomAllocation).Returns(_roomAllocationRepositoryMock.Object);

        _billingRepositoryMock
            .Setup(b => b.GetTableData<GMS.Infrastructure.Models.Guests.BillingDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new List<GMS.Infrastructure.Models.Guests.BillingDTO>());
        _billingRepositoryMock
            .Setup(b => b.RunSQLCommand(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(true);

        _roomAllocationRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<RoomAllocation>()))
            .ReturnsAsync(true);

        _membersDetailsRepositoryMock
            .Setup(m => m.IsExists(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(true);

        _membersDetailsRepositoryMock
            .Setup(m => m.UpdateAsync(It.IsAny<MembersDetails>()))
            .ReturnsAsync(true);

        var logger = Mock.Of<ILogger<GuestsAPIController>>();
        var hostingEnv = Mock.Of<IWebHostEnvironment>();

        _controller = new GuestsAPIController(_unitOfWorkMock.Object, logger, _mapper, hostingEnv);
    }

    [Fact]
    public async Task ManageGuests_ReturnsOk_WhenGuestEligible()
    {
        // Arrange
        var existingMember = CreateMemberDetailsDto();
        _membersDetailsRepositoryMock
            .Setup(m => m.GetEntityData<MembersDetailsDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(existingMember);

        var roomAllocation = new RoomAllocation
        {
            GuestId = existingMember.Id,
            CheckOutDate = null
        };

        _genOperationsRepositoryMock
            .Setup(g => g.GetEntityData<RoomAllocation>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(roomAllocation);

        _genOperationsRepositoryMock
            .Setup(g => g.IsExists(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(false);

        var input = new MembersDetailsDTO
        {
            GroupId = existingMember.GroupId,
            PAXSno = existingMember.PAXSno,
            MobileNo = existingMember.MobileNo,
            RoomType = 1,
            DateOfArrival = DateTime.Today,
            DateOfDepartment = DateTime.Today.AddDays(1)
        };

        // Act
        var result = await _controller.ManageGuests(input);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<MembersDetailsDTO>();
        _membersDetailsRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<MembersDetails>()), Times.Once);
        _genOperationsRepositoryMock.Verify(g => g.IsExists(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        _roomAllocationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RoomAllocation>()), Times.Once);
    }

    [Fact]
    public async Task ManageGuests_ReturnsBadRequest_WhenGuestAlreadyCheckedOut()
    {
        // Arrange
        var existingMember = CreateMemberDetailsDto();
        _membersDetailsRepositoryMock
            .Setup(m => m.GetEntityData<MembersDetailsDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(existingMember);

        var roomAllocation = new RoomAllocation
        {
            GuestId = existingMember.Id,
            CheckOutDate = DateTime.UtcNow
        };

        _genOperationsRepositoryMock
            .Setup(g => g.GetEntityData<RoomAllocation>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(roomAllocation);

        var input = new MembersDetailsDTO
        {
            GroupId = existingMember.GroupId,
            PAXSno = existingMember.PAXSno
        };

        // Act
        var result = await _controller.ManageGuests(input);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Guest is already checked out");
        _membersDetailsRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<MembersDetails>()), Times.Never);
    }

    [Fact]
    public async Task ManageGuests_ReturnsBadRequest_WhenSettlementExists()
    {
        // Arrange
        var existingMember = CreateMemberDetailsDto();
        _membersDetailsRepositoryMock
            .Setup(m => m.GetEntityData<MembersDetailsDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(existingMember);

        var roomAllocation = new RoomAllocation
        {
            GuestId = existingMember.Id,
            CheckOutDate = null
        };

        _genOperationsRepositoryMock
            .Setup(g => g.GetEntityData<RoomAllocation>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(roomAllocation);

        _genOperationsRepositoryMock
            .Setup(g => g.IsExists(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(true);

        var input = new MembersDetailsDTO
        {
            GroupId = existingMember.GroupId,
            PAXSno = existingMember.PAXSno
        };

        // Act
        var result = await _controller.ManageGuests(input);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Settlement already exists for this guest");
        _membersDetailsRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<MembersDetails>()), Times.Never);
    }

    private static MembersDetailsDTO CreateMemberDetailsDto()
    {
        return new MembersDetailsDTO
        {
            Id = 7,
            GroupId = "GROUP-1",
            PAXSno = 1,
            MobileNo = "9999999999",
            IsCrm = 1,
            Status = 1,
            CreationDate = DateTime.UtcNow.AddDays(-2),
            AprovedDate = DateTime.UtcNow.AddDays(-1),
            IsApproved = 1,
            ApprovedBy = 22,
            UniqueNo = "U-1",
            Photo = "photo.jpg"
        };
    }
}
