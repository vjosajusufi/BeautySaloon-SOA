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

public class SalonServiceServiceAdditionalTests
{
    private readonly IServiceRepository _repository;
    private readonly SalonServiceService _sut;

    public SalonServiceServiceAdditionalTests()
    {
        _repository = Substitute.For<IServiceRepository>();
        var cache   = new MemoryCache(new MemoryCacheOptions());
        var mapper  = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _sut        = new SalonServiceService(_repository, cache, mapper);
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsMappedDto()
    {
        // Arrange
        var service = new Service { Id = 3, Name = "Facial", Description = "Deep clean", Price = 35m, DurationMinutes = 60, IsActive = true };
        _repository.GetById(3, Arg.Any<CancellationToken>()).Returns(service);

        // Act
        var result = await _sut.GetById(3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result!.Id);
        Assert.Equal("Facial", result.Name);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((Service?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.Null(result);
    }
}
