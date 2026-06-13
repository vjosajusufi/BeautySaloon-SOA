using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeautySaloon.Tests.Repositories;

public class AppointmentRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static (User user, Service service) SeedUserAndService(AppDbContext ctx)
    {
        var user = new User
        {
            FirstName    = "Jane",
            LastName     = "Doe",
            Email        = $"jane{Guid.NewGuid():N}@example.com",
            PasswordHash = "hash",
            Role         = "Client"
        };
        var service = new Service
        {
            Name            = "Haircut",
            Description     = string.Empty,
            Price           = 15m,
            DurationMinutes = 30,
            IsActive        = true
        };
        ctx.Users.Add(user);
        ctx.Services.Add(service);
        ctx.SaveChanges();
        return (user, service);
    }

    [Fact]
    public async Task GetAll_ReturnsAllAppointmentsWithNavigationProperties()
    {
        // Arrange
        await using var ctx = CreateContext();
        var (user, service) = SeedUserAndService(ctx);

        ctx.Appointments.AddRange(
            new Appointment { UserId = user.Id, ServiceId = service.Id, AppointmentDate = new DateOnly(2025, 6, 2), StartTime = new TimeOnly(9,  0), EndTime = new TimeOnly(9,  30), Status = "Pending" },
            new Appointment { UserId = user.Id, ServiceId = service.Id, AppointmentDate = new DateOnly(2025, 6, 3), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 30), Status = "Confirmed" }
        );
        await ctx.SaveChangesAsync();

        var repo = new AppointmentRepository(ctx);

        // Act
        var result = (await repo.GetAll()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, a =>
        {
            Assert.NotNull(a.User);
            Assert.NotNull(a.Service);
        });
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsAppointmentWithNavigationProperties()
    {
        // Arrange
        await using var ctx = CreateContext();
        var (user, service) = SeedUserAndService(ctx);

        var appointment = new Appointment
        {
            UserId          = user.Id,
            ServiceId       = service.Id,
            AppointmentDate = new DateOnly(2025, 6, 2),
            StartTime       = new TimeOnly(10, 0),
            EndTime         = new TimeOnly(10, 30),
            Status          = "Pending",
            Notes           = "Window seat please"
        };
        ctx.Appointments.Add(appointment);
        await ctx.SaveChangesAsync();

        var repo = new AppointmentRepository(ctx);

        // Act
        var result = await repo.GetById(appointment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(appointment.Id, result!.Id);
        Assert.Equal("Window seat please", result.Notes);
        Assert.NotNull(result.User);
        Assert.NotNull(result.Service);
        Assert.Equal("Haircut", result.Service.Name);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNull()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new AppointmentRepository(ctx);

        // Act
        var result = await repo.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_PersistsAppointmentAndReturnsWithId()
    {
        // Arrange
        await using var ctx = CreateContext();
        var (user, service) = SeedUserAndService(ctx);

        var repo        = new AppointmentRepository(ctx);
        var appointment = new Appointment
        {
            UserId          = user.Id,
            ServiceId       = service.Id,
            AppointmentDate = new DateOnly(2025, 7, 14),
            StartTime       = new TimeOnly(14, 0),
            EndTime         = new TimeOnly(14, 30),
            Status          = "Pending",
            Notes           = "Referral"
        };

        // Act
        var result = await repo.Create(appointment);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(1, await ctx.Appointments.CountAsync());
        var persisted = await ctx.Appointments.FindAsync(result.Id);
        Assert.NotNull(persisted);
        Assert.Equal("Referral", persisted!.Notes);
        Assert.Equal(new TimeOnly(14, 0), persisted.StartTime);
    }
}
