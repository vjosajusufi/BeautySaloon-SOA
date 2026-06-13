using System.Security.Claims;
using BeautySaloon_API.Controllers;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BeautySaloon.Tests.Controllers;

public class AppointmentsControllerAdditionalTests
{
    private readonly IAppointmentService _service;
    private readonly AppointmentsController _sut;

    public AppointmentsControllerAdditionalTests()
    {
        _service = Substitute.For<IAppointmentService>();
        _sut     = new AppointmentsController(_service);
    }

    private static ClaimsPrincipal MakeUser(int id) =>
        new(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }));

    [Fact]
    public async Task Create_WhenValid_ReturnsCreatedAtAction()
    {
        // Arrange
        var req = new CreateAppointmentDto { UserId = 1, ServiceId = 1, AppointmentDate = new DateOnly(2025, 6, 2), StartTime = new TimeOnly(10, 0) };
        var dto = new AppointmentDto { Id = 7, UserId = 1, ServiceId = 1, Status = "Pending" };
        _service.Create(req, Arg.Any<CancellationToken>()).Returns(dto);

        // Act
        var result = await _sut.Create(req);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(7, ((AppointmentDto)created.Value!).Id);
    }

    [Fact]
    public async Task Create_WhenServiceNotFound_ReturnsNotFound()
    {
        // Arrange
        var req = new CreateAppointmentDto { ServiceId = 99 };
        _service.Create(req, Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException("Service not found."));

        // Act
        var result = await _sut.Create(req);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_WhenSlotConflict_ReturnsBadRequest()
    {
        // Arrange
        var req = new CreateAppointmentDto { ServiceId = 1 };
        _service.Create(req, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("This time slot is already booked."));

        // Act
        var result = await _sut.Create(req);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenFound_ReturnsOk()
    {
        // Arrange
        var req     = new CreateAppointmentDto { UserId = 1, ServiceId = 1, AppointmentDate = new DateOnly(2025, 7, 1), StartTime = new TimeOnly(11, 0) };
        var updated = new AppointmentDto { Id = 3, UserId = 1, ServiceId = 1, Status = "Confirmed" };
        _service.Update(3, req, Arg.Any<CancellationToken>()).Returns(updated);

        // Act
        var result = await _sut.Update(3, req);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Confirmed", ((AppointmentDto)ok.Value!).Status);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var req = new CreateAppointmentDto { ServiceId = 1 };
        _service.Update(99, req, Arg.Any<CancellationToken>()).Returns((AppointmentDto?)null);

        // Act
        var result = await _sut.Update(99, req);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_WhenInvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        var req = new CreateAppointmentDto { ServiceId = 1 };
        _service.Update(1, req, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Conflict."));

        // Act
        var result = await _sut.Update(1, req);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenServiceNotFound_ReturnsNotFound()
    {
        // Arrange
        var req = new CreateAppointmentDto { ServiceId = 999 };
        _service.Update(1, req, Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException("Service not found."));

        // Act
        var result = await _sut.Update(1, req);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetMy_WhenClaimPresent_ReturnsOk()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = MakeUser(3) }
        };
        var list = new List<AppointmentDto> { new() { Id = 1, UserId = 3 } };
        _service.GetByUserId(3, Arg.Any<CancellationToken>()).Returns(list);

        // Act
        var result = await _sut.GetMy();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single((IEnumerable<AppointmentDto>)ok.Value!);
    }

    [Fact]
    public async Task GetMy_WhenNoClaim_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await _sut.GetMy();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
}
