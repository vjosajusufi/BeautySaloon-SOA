using BeautySaloon_API.Controllers;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BeautySaloon.Tests.Controllers;

public class AppointmentsControllerTests
{
    private readonly IAppointmentService _service;
    private readonly AppointmentsController _sut;

    public AppointmentsControllerTests()
    {
        _service = Substitute.For<IAppointmentService>();
        _sut     = new AppointmentsController(_service);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new() { Id = 1, UserId = 1, ServiceId = 1, UserFullName = "Jane Doe",  ServiceName = "Haircut",  Status = "Pending" },
            new() { Id = 2, UserId = 2, ServiceId = 2, UserFullName = "John Smith", ServiceName = "Manicure", Status = "Confirmed" }
        };
        _service.GetAll(Arg.Any<CancellationToken>()).Returns(appointments);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var ok       = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(ok.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        // Arrange
        var dto = new AppointmentDto
        {
            Id          = 1,
            UserId      = 1,
            ServiceId   = 1,
            UserFullName = "Jane Doe",
            ServiceName = "Haircut",
            Status      = "Pending",
            AppointmentDate = new DateOnly(2025, 6, 2),
            StartTime   = new TimeOnly(10, 0),
            EndTime     = new TimeOnly(10, 30)
        };
        _service.GetById(1, Arg.Any<CancellationToken>()).Returns(dto);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var ok       = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<AppointmentDto>(ok.Value);
        Assert.Equal(1, returned.Id);
        Assert.Equal("Jane Doe", returned.UserFullName);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _service.GetById(99, Arg.Any<CancellationToken>()).Returns((AppointmentDto?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContent()
    {
        // Arrange
        _service.Delete(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _service.Delete(99, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Delete(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
