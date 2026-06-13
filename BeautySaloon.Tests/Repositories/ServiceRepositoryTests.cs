using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeautySaloon.Tests.Repositories;

public class ServiceRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetAll_ReturnsAllActiveServices()
    {
        // Arrange
        await using var context = CreateContext();
        context.Services.AddRange(
            new Service { Name = "Haircut",          Description = string.Empty, Price = 15m, DurationMinutes = 30,  IsActive = true },
            new Service { Name = "Manicure",         Description = string.Empty, Price = 20m, DurationMinutes = 45,  IsActive = true },
            new Service { Name = "Facial Treatment", Description = string.Empty, Price = 35m, DurationMinutes = 60,  IsActive = true }
        );
        await context.SaveChangesAsync();
        var repo = new ServiceRepository(context);

        // Act
        var result = await repo.GetAll();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.All(result, s => Assert.True(s.IsActive));
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsService()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new Service
        {
            Name = "Haircut", Description = string.Empty,
            Price = 15m, DurationMinutes = 30, IsActive = true
        };
        context.Services.Add(service);
        await context.SaveChangesAsync();
        var repo = new ServiceRepository(context);

        // Act
        var result = await repo.GetById(service.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Haircut", result.Name);
        Assert.Equal(15m, result.Price);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNull()
    {
        // Arrange
        await using var context = CreateContext();
        var repo = new ServiceRepository(context);

        // Act
        var result = await repo.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_AddsServiceToDatabase()
    {
        // Arrange
        await using var context = CreateContext();
        var repo = new ServiceRepository(context);
        var service = new Service
        {
            Name = "Pedicure", Description = string.Empty,
            Price = 25m, DurationMinutes = 60, IsActive = true
        };

        // Act
        var result = await repo.Create(service);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(1, await context.Services.CountAsync());
        var persisted = await context.Services.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal("Pedicure", persisted.Name);
        Assert.Equal(25m, persisted.Price);
    }
}
