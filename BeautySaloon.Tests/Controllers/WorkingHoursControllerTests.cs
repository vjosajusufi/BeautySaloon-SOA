using BeautySaloon_API.Controllers;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BeautySaloon.Tests.Controllers;

public class WorkingHoursControllerTests
{
    private readonly IWorkingHoursService _service;
    private readonly WorkingHoursController _sut;

    public WorkingHoursControllerTests()
    {
        _service = Substitute.For<IWorkingHoursService>();
        _sut = new WorkingHoursController(_service);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<WorkingHoursDto>
        {
            new() { Id = 1, DayOfWeek = DayOfWeek.Monday,  OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0), IsOpen = true },
            new() { Id = 2, DayOfWeek = DayOfWeek.Sunday,  OpenTime = new TimeOnly(0, 0), CloseTime = new TimeOnly(0, 0),  IsOpen = false }
        };
        _service.GetAll(Arg.Any<CancellationToken>()).Returns(list);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(2, ((IEnumerable<WorkingHoursDto>)ok.Value!).Count());
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsOk()
    {
        // Arrange
        var dto = new WorkingHoursDto { Id = 1, DayOfWeek = DayOfWeek.Monday, IsOpen = true };
        _service.GetById(1, Arg.Any<CancellationToken>()).Returns(dto);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, ((WorkingHoursDto)ok.Value!).Id);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _service.GetById(99, Arg.Any<CancellationToken>()).Returns((WorkingHoursDto?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WhenSuccessful_ReturnsCreated()
    {
        // Arrange
        var req     = new CreateWorkingHoursDto { DayOfWeek = DayOfWeek.Friday, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(17, 0), IsOpen = true };
        var created = new WorkingHoursDto { Id = 5, DayOfWeek = DayOfWeek.Friday, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(17, 0), IsOpen = true };
        _service.Create(req, Arg.Any<CancellationToken>()).Returns(created);

        // Act
        var result = await _sut.Create(req);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(5, ((WorkingHoursDto)createdResult.Value!).Id);
    }

    [Fact]
    public async Task Create_WhenDayAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var req = new CreateWorkingHoursDto { DayOfWeek = DayOfWeek.Monday };
        _service.Create(req, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Working hours for Monday already exist."));

        // Act
        var result = await _sut.Create(req);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenFound_ReturnsOk()
    {
        // Arrange
        var req     = new CreateWorkingHoursDto { DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(16, 0), IsOpen = true };
        var updated = new WorkingHoursDto { Id = 1, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(16, 0), IsOpen = true };
        _service.Update(1, req, Arg.Any<CancellationToken>()).Returns(updated);

        // Act
        var result = await _sut.Update(1, req);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(new TimeOnly(8, 0), ((WorkingHoursDto)ok.Value!).OpenTime);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var req = new CreateWorkingHoursDto { DayOfWeek = DayOfWeek.Monday };
        _service.Update(99, req, Arg.Any<CancellationToken>()).Returns((WorkingHoursDto?)null);

        // Act
        var result = await _sut.Update(99, req);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
