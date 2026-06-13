using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeautySaloon.Tests.Repositories;

public class AppointmentRepositoryAdditionalTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static (User user, Service service) SeedUserAndService(AppDbContext ctx, string email = "u@u.com")
    {
        var user    = new User { FirstName = "A", LastName = "B", Email = email, PasswordHash = "h", Role = "Client" };
        var service = new Service { Name = "Haircut", Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true };
        ctx.Users.Add(user);
        ctx.Services.Add(service);
        ctx.SaveChanges();
        return (user, service);
    }

    [Fact]
    public async Task GetByUserId_ReturnsOnlyThatUsersAppointments()
    {
        // Arrange
        await using var ctx = CreateContext();
        var (u1, svc) = SeedUserAndService(ctx, "u1@u.com");
        var u2        = new User { FirstName = "C", LastName = "D", Email = "u2@u.com", PasswordHash = "h", Role = "Client" };
        ctx.Users.Add(u2);
        await ctx.SaveChangesAsync();

        ctx.Appointments.AddRange(
            new Appointment { UserId = u1.Id, ServiceId = svc.Id, AppointmentDate = new DateOnly(2025, 6, 2), StartTime = new TimeOnly(9,  0), EndTime = new TimeOnly(9,  30), Status = "Pending" },
            new Appointment { UserId = u1.Id, ServiceId = svc.Id, AppointmentDate = new DateOnly(2025, 6, 3), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30), Status = "Pending" },
            new Appointment { UserId = u2.Id, ServiceId = svc.Id, AppointmentDate = new DateOnly(2025, 6, 4), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 30), Status = "Pending" }
        );
        await ctx.SaveChangesAsync();
        var repo = new AppointmentRepository(ctx);

        // Act
        var result = (await repo.GetByUserId(u1.Id)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal(u1.Id, a.UserId));
    }

    [Fact]
    public async Task Update_PersistsChanges()
    {
        // Arrange
        await using var ctx   = CreateContext();
        var (user, svc) = SeedUserAndService(ctx);

        var appt = new Appointment
        {
            UserId          = user.Id, ServiceId = svc.Id,
            AppointmentDate = new DateOnly(2025, 7, 7),
            StartTime       = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30),
            Status          = "Pending"
        };
        ctx.Appointments.Add(appt);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var toUpdate = new Appointment
        {
            Id              = appt.Id,
            UserId          = user.Id, ServiceId = svc.Id,
            AppointmentDate = new DateOnly(2025, 7, 7),
            StartTime       = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 30),
            Status          = "Confirmed"
        };
        var repo = new AppointmentRepository(ctx);

        // Act
        var result = await repo.Update(toUpdate);

        // Assert
        Assert.Equal("Confirmed", result.Status);
        var persisted = await ctx.Appointments.FindAsync(appt.Id);
        Assert.Equal("Confirmed", persisted!.Status);
        Assert.Equal(new TimeOnly(11, 0), persisted.StartTime);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesAndReturnsTrue()
    {
        // Arrange
        await using var ctx   = CreateContext();
        var (user, svc) = SeedUserAndService(ctx);

        var appt = new Appointment
        {
            UserId = user.Id, ServiceId = svc.Id,
            AppointmentDate = new DateOnly(2025, 8, 1),
            StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 30), Status = "Pending"
        };
        ctx.Appointments.Add(appt);
        await ctx.SaveChangesAsync();
        var repo = new AppointmentRepository(ctx);

        // Act
        var result = await repo.Delete(appt.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await ctx.Appointments.CountAsync());
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new AppointmentRepository(ctx);

        // Act
        var result = await repo.Delete(999);

        // Assert
        Assert.False(result);
    }
}
