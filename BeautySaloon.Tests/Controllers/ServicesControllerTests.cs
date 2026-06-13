using BeautySaloon_API.Controllers;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BeautySaloon.Tests.Controllers;

public class ServicesControllerTests
{
    private readonly ISalonServiceService _service;
    private readonly ServicesController _sut;

    public ServicesControllerTests()
    {
        _service = Substitute.For<ISalonServiceService>();
        _sut = new ServicesController(_service);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        var services = new List<ServiceDto>
        {
            new() { Id = 1, Name = "Haircut",  Price = 15m, DurationMinutes = 30,  IsActive = true },
            new() { Id = 2, Name = "Manicure", Price = 20m, DurationMinutes = 45,  IsActive = true },
            new() { Id = 3, Name = "Pedicure", Price = 25m, DurationMinutes = 60,  IsActive = true }
        };
        _service.GetAll(Arg.Any<CancellationToken>()).Returns(services);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<ServiceDto>>(ok.Value);
        Assert.Equal(3, returned.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        // Arrange
        var dto = new ServiceDto { Id = 1, Name = "Haircut", Price = 15m, DurationMinutes = 30, IsActive = true };
        _service.GetById(1, Arg.Any<CancellationToken>()).Returns(dto);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<ServiceDto>(ok.Value);
        Assert.Equal(1, returned.Id);
        Assert.Equal("Haircut", returned.Name);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _service.GetById(99, Arg.Any<CancellationToken>()).Returns((ServiceDto?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WhenValid_ReturnsCreated()
    {
        // Arrange
        var request = new CreateServiceDto
        {
            Name = "Facial Treatment", Description = "Deep cleansing",
            Price = 35m, DurationMinutes = 60
        };
        var created = new ServiceDto
        {
            Id = 5, Name = "Facial Treatment", Description = "Deep cleansing",
            Price = 35m, DurationMinutes = 60, IsActive = true
        };
        _service.Create(request, Arg.Any<CancellationToken>()).Returns(created);

        // Act
        var result = await _sut.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(nameof(_sut.GetById), createdResult.ActionName);
        var returned = Assert.IsType<ServiceDto>(createdResult.Value);
        Assert.Equal(5, returned.Id);
        Assert.Equal("Facial Treatment", returned.Name);
    }
}
