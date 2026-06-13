using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeautySaloon.Tests.Repositories;

public class ServiceRepositoryAdditionalTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Update_PersistsChanges()
    {
        // Arrange
        await using var ctx = CreateContext();
        var service = new Service { Name = "Haircut", Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true };
        ctx.Services.Add(service);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var toUpdate = new Service { Id = service.Id, Name = "Haircut Premium", Description = "Updated", Price = 25m, DurationMinutes = 45, IsActive = true };
        var repo     = new ServiceRepository(ctx);

        // Act
        var result = await repo.Update(toUpdate);

        // Assert
        Assert.Equal("Haircut Premium", result.Name);
        var persisted = await ctx.Services.FindAsync(service.Id);
        Assert.Equal(25m, persisted!.Price);
        Assert.Equal(45, persisted.DurationMinutes);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesServiceAndReturnsTrue()
    {
        // Arrange
        await using var ctx = CreateContext();
        var service = new Service { Name = "Manicure", Description = string.Empty, Price = 20m, DurationMinutes = 45, IsActive = true };
        ctx.Services.Add(service);
        await ctx.SaveChangesAsync();
        var repo = new ServiceRepository(ctx);

        // Act
        var result = await repo.Delete(service.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await ctx.Services.CountAsync());
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new ServiceRepository(ctx);

        // Act
        var result = await repo.Delete(999);

        // Assert
        Assert.False(result);
    }
}
