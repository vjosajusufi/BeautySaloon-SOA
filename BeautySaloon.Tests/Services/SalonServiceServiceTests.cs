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

public class SalonServiceServiceTests
{
    private readonly IServiceRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;
    private readonly SalonServiceService _sut;

    public SalonServiceServiceTests()
    {
        _repository = Substitute.For<IServiceRepository>();
        _cache      = new MemoryCache(new MemoryCacheOptions());
        _mapper     = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _sut        = new SalonServiceService(_repository, _cache, _mapper);
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyActiveServices()
    {
        // Arrange
        var services = new List<Service>
        {
            new() { Id = 1, Name = "Haircut",  Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true  },
            new() { Id = 2, Name = "Waxing",   Description = string.Empty, Price = 10m, DurationMinutes = 20, IsActive = false },
            new() { Id = 3, Name = "Manicure", Description = string.Empty, Price = 20m, DurationMinutes = 45, IsActive = true  }
        };
        _repository.GetAll(Arg.Any<CancellationToken>()).Returns(services);

        // Act
        var result = (await _sut.GetAll()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.True(s.IsActive));
        Assert.DoesNotContain(result, s => s.Name == "Waxing");
    }

    [Fact]
    public async Task Create_AddsServiceAndInvalidatesCache()
    {
        // Arrange — prime the cache so we can verify it is cleared
        _cache.Set("services:all:v1", new List<ServiceDto> { new() { Id = 99 } });

        var dto      = new CreateServiceDto { Name = "Pedicure", Description = string.Empty, Price = 25m, DurationMinutes = 60 };
        var created  = new Service { Id = 4, Name = "Pedicure", Description = string.Empty, Price = 25m, DurationMinutes = 60, IsActive = true };
        _repository.Create(Arg.Any<Service>(), Arg.Any<CancellationToken>()).Returns(created);

        // Act
        var result = await _sut.Create(dto);

        // Assert
        Assert.Equal(4, result.Id);
        Assert.Equal("Pedicure", result.Name);
        Assert.False(_cache.TryGetValue("services:all:v1", out _)); // cache invalidated
    }

    [Fact]
    public async Task Update_WhenServiceExists_ReturnsUpdatedDto()
    {
        // Arrange
        var existing = new Service { Id = 1, Name = "Old Name", Description = string.Empty, Price = 10m, DurationMinutes = 20, IsActive = true };
        var updated  = new Service { Id = 1, Name = "New Name", Description = "Updated",     Price = 15m, DurationMinutes = 30, IsActive = true };
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.Update(Arg.Any<Service>(), Arg.Any<CancellationToken>()).Returns(updated);

        var dto = new CreateServiceDto { Name = "New Name", Description = "Updated", Price = 15m, DurationMinutes = 30 };

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result!.Name);
        Assert.Equal(15m, result.Price);
    }

    [Fact]
    public async Task Update_WhenServiceNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((Service?)null);

        // Act
        var result = await _sut.Update(99, new CreateServiceDto());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_WhenServiceExists_ReturnsTrueAndInvalidatesCache()
    {
        // Arrange
        _cache.Set("services:all:v1", new List<ServiceDto>());
        _repository.Delete(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.True(result);
        Assert.False(_cache.TryGetValue("services:all:v1", out _));
    }

    [Fact]
    public async Task Delete_WhenServiceNotFound_ReturnsFalse()
    {
        // Arrange
        _repository.Delete(99, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Delete(99);

        // Assert
        Assert.False(result);
    }
}
