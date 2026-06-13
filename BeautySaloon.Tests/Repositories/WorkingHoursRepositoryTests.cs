using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeautySaloon.Tests.Repositories;

public class WorkingHoursRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static WorkingHours MakeWh(DayOfWeek day, bool isOpen = true) =>
        new() { DayOfWeek = day, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0), IsOpen = isOpen };

    [Fact]
    public async Task GetAll_ReturnsAllOrderedByDayOfWeek()
    {
        // Arrange
        await using var ctx = CreateContext();
        ctx.WorkingHours.AddRange(
            MakeWh(DayOfWeek.Wednesday),
            MakeWh(DayOfWeek.Monday),
            MakeWh(DayOfWeek.Friday)
        );
        await ctx.SaveChangesAsync();
        var repo = new WorkingHoursRepository(ctx);

        // Act
        var result = (await repo.GetAll()).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        // DayOfWeek enum: Sunday=0, Monday=1, Wednesday=3, Friday=5
        Assert.True(result[0].DayOfWeek <= result[1].DayOfWeek);
        Assert.True(result[1].DayOfWeek <= result[2].DayOfWeek);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsWorkingHours()
    {
        // Arrange
        await using var ctx = CreateContext();
        var wh = MakeWh(DayOfWeek.Tuesday);
        ctx.WorkingHours.Add(wh);
        await ctx.SaveChangesAsync();
        var repo = new WorkingHoursRepository(ctx);

        // Act
        var result = await repo.GetById(wh.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DayOfWeek.Tuesday, result!.DayOfWeek);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new WorkingHoursRepository(ctx);

        // Act
        var result = await repo.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByDayOfWeek_WhenExists_ReturnsWorkingHours()
    {
        // Arrange
        await using var ctx = CreateContext();
        ctx.WorkingHours.Add(MakeWh(DayOfWeek.Saturday));
        await ctx.SaveChangesAsync();
        var repo = new WorkingHoursRepository(ctx);

        // Act
        var result = await repo.GetByDayOfWeek(DayOfWeek.Saturday);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DayOfWeek.Saturday, result!.DayOfWeek);
    }

    [Fact]
    public async Task GetByDayOfWeek_WhenNotFound_ReturnsNull()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new WorkingHoursRepository(ctx);

        // Act
        var result = await repo.GetByDayOfWeek(DayOfWeek.Sunday);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_PersistsWorkingHoursAndReturnsWithId()
    {
        // Arrange
        await using var ctx  = CreateContext();
        var repo = new WorkingHoursRepository(ctx);
        var wh   = MakeWh(DayOfWeek.Thursday);

        // Act
        var result = await repo.Create(wh);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(1, await ctx.WorkingHours.CountAsync());
    }

    [Fact]
    public async Task Update_PersistsChanges()
    {
        // Arrange
        await using var ctx = CreateContext();
        var wh = MakeWh(DayOfWeek.Monday);
        ctx.WorkingHours.Add(wh);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var toUpdate = new WorkingHours
        {
            Id        = wh.Id,
            DayOfWeek = DayOfWeek.Monday,
            OpenTime  = new TimeOnly(8, 0),
            CloseTime = new TimeOnly(16, 0),
            IsOpen    = true
        };
        var repo = new WorkingHoursRepository(ctx);

        // Act
        var result = await repo.Update(toUpdate);

        // Assert
        Assert.Equal(new TimeOnly(8, 0), result.OpenTime);
        var persisted = await ctx.WorkingHours.FindAsync(wh.Id);
        Assert.Equal(new TimeOnly(8, 0), persisted!.OpenTime);
    }
}
