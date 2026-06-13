using AutoMapper;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Helpers;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Xunit;

namespace BeautySaloon.Tests.Services;

public class WorkingHoursServiceTests
{
    private readonly IWorkingHoursRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly WorkingHoursService _sut;

    public WorkingHoursServiceTests()
    {
        _repository = Substitute.For<IWorkingHoursRepository>();
        _cache      = new MemoryCache(new MemoryCacheOptions());
        var mapper  = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _sut        = new WorkingHoursService(_repository, _cache, mapper);
    }

    private static WorkingHours MakeWh(int id, DayOfWeek day, bool isOpen = true) =>
        new() { Id = id, DayOfWeek = day, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0), IsOpen = isOpen };

    [Fact]
    public async Task GetAll_WhenCacheMiss_FetchesFromRepositoryAndCaches()
    {
        // Arrange
        var data = new[] { MakeWh(1, DayOfWeek.Monday), MakeWh(2, DayOfWeek.Tuesday) };
        _repository.GetAll(Arg.Any<CancellationToken>()).Returns(data);

        // Act
        var result = (await _sut.GetAll()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        await _repository.Received(1).GetAll(Arg.Any<CancellationToken>());
        Assert.True(_cache.TryGetValue("workinghours:all:v1", out _));
    }

    [Fact]
    public async Task GetAll_WhenCacheHit_DoesNotCallRepository()
    {
        // Arrange — prime cache manually
        var cached = new List<WorkingHoursDto> { new() { Id = 1, DayOfWeek = DayOfWeek.Monday } };
        _cache.Set("workinghours:all:v1", (IEnumerable<WorkingHoursDto>)cached);

        // Act
        var result = (await _sut.GetAll()).ToList();

        // Assert
        Assert.Single(result);
        await _repository.DidNotReceive().GetAll(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsMappedDto()
    {
        // Arrange
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(MakeWh(1, DayOfWeek.Monday));

        // Act
        var result = await _sut.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DayOfWeek.Monday, result!.DayOfWeek);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((WorkingHours?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByDayOfWeek_WhenFound_ReturnsMappedDto()
    {
        // Arrange
        _repository.GetByDayOfWeek(DayOfWeek.Friday, Arg.Any<CancellationToken>())
            .Returns(MakeWh(5, DayOfWeek.Friday));

        // Act
        var result = await _sut.GetByDayOfWeek(DayOfWeek.Friday);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DayOfWeek.Friday, result!.DayOfWeek);
    }

    [Fact]
    public async Task GetByDayOfWeek_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetByDayOfWeek(DayOfWeek.Sunday, Arg.Any<CancellationToken>()).Returns((WorkingHours?)null);

        // Act
        var result = await _sut.GetByDayOfWeek(DayOfWeek.Sunday);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_WhenDayDoesNotExist_CreatesAndInvalidatesCache()
    {
        // Arrange
        _cache.Set("workinghours:all:v1", new List<WorkingHoursDto>());
        _repository.GetByDayOfWeek(DayOfWeek.Wednesday, Arg.Any<CancellationToken>()).Returns((WorkingHours?)null);
        var created = MakeWh(3, DayOfWeek.Wednesday);
        _repository.Create(Arg.Any<WorkingHours>(), Arg.Any<CancellationToken>()).Returns(created);

        var dto = new CreateWorkingHoursDto { DayOfWeek = DayOfWeek.Wednesday, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0), IsOpen = true };

        // Act
        var result = await _sut.Create(dto);

        // Assert
        Assert.Equal(DayOfWeek.Wednesday, result.DayOfWeek);
        Assert.False(_cache.TryGetValue("workinghours:all:v1", out _));
    }

    [Fact]
    public async Task Create_WhenDayAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        _repository.GetByDayOfWeek(DayOfWeek.Monday, Arg.Any<CancellationToken>())
            .Returns(MakeWh(1, DayOfWeek.Monday));

        var dto = new CreateWorkingHoursDto { DayOfWeek = DayOfWeek.Monday };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task Update_WhenFound_UpdatesAndInvalidatesCache()
    {
        // Arrange
        _cache.Set("workinghours:all:v1", new List<WorkingHoursDto>());
        var existing = MakeWh(1, DayOfWeek.Monday);
        var afterUpd = new WorkingHours { Id = 1, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(17, 0), IsOpen = true };
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.Update(Arg.Any<WorkingHours>(), Arg.Any<CancellationToken>()).Returns(afterUpd);

        var dto = new CreateWorkingHoursDto { DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(17, 0), IsOpen = true };

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new TimeOnly(8, 0), result!.OpenTime);
        Assert.False(_cache.TryGetValue("workinghours:all:v1", out _));
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((WorkingHours?)null);

        // Act
        var result = await _sut.Update(99, new CreateWorkingHoursDto());

        // Assert
        Assert.Null(result);
    }
}
