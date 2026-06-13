using BeautySaloon_API.Controllers;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BeautySaloon.Tests.Controllers;

public class ServicesControllerAdditionalTests
{
    private readonly ISalonServiceService _service;
    private readonly ServicesController _sut;

    public ServicesControllerAdditionalTests()
    {
        _service = Substitute.For<ISalonServiceService>();
        _sut     = new ServicesController(_service);
    }

    [Fact]
    public async Task Update_WhenFound_ReturnsOk()
    {
        // Arrange
        var req     = new CreateServiceDto { Name = "Updated", Description = "Desc", Price = 30m, DurationMinutes = 45 };
        var updated = new ServiceDto { Id = 2, Name = "Updated", Price = 30m, DurationMinutes = 45, IsActive = true };
        _service.Update(2, req, Arg.Any<CancellationToken>()).Returns(updated);

        // Act
        var result = await _sut.Update(2, req);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Updated", ((ServiceDto)ok.Value!).Name);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var req = new CreateServiceDto { Name = "X" };
        _service.Update(99, req, Arg.Any<CancellationToken>()).Returns((ServiceDto?)null);

        // Act
        var result = await _sut.Update(99, req);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenFound_ReturnsNoContent()
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
