using AutoMapper;
using BeautySaloon_API.Data;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Helpers;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace BeautySaloon.Tests.Services;

public class AppointmentServiceAdditionalTests : IDisposable
{
    private readonly IAppointmentRepository _repository;
    private readonly AppDbContext _context;
    private readonly AppointmentService _sut;

    private static readonly DateOnly Monday = new(2025, 6, 2); // Monday

    public AppointmentServiceAdditionalTests()
    {
        _repository = Substitute.For<IAppointmentRepository>();

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(opts);

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _sut       = new AppointmentService(_repository, _context, mapper);
    }

    private async Task SeedBaseDataAsync()
    {
        _context.Services.Add(new Service { Id = 1, Name = "Haircut", Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true });
        _context.WorkingHours.Add(new WorkingHours { Id = 1, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0), IsOpen = true });
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ReturnsMappedDtos()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new()
            {
                Id = 1, UserId = 1, ServiceId = 1, AppointmentDate = Monday,
                StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30), Status = "Pending",
                User    = new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", Role = "Client" },
                Service = new Service { Id = 1, Name = "Haircut", Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true }
            }
        };
        _repository.GetAll(Arg.Any<CancellationToken>()).Returns(appointments);

        // Act
        var result = (await _sut.GetAll()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Haircut", result[0].ServiceName);
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsMappedDto()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 2, UserId = 1, ServiceId = 1, AppointmentDate = Monday,
            StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 30), Status = "Confirmed",
            User    = new User { Id = 1, FirstName = "X", LastName = "Y", Email = "x@y.com", Role = "Client" },
            Service = new Service { Id = 1, Name = "Haircut", Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true }
        };
        _repository.GetById(2, Arg.Any<CancellationToken>()).Returns(appointment);

        // Act
        var result = await _sut.GetById(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Id);
        Assert.Equal("Confirmed", result.Status);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((Appointment?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserId_ReturnsMappedDtos()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new()
            {
                Id = 3, UserId = 5, ServiceId = 1, AppointmentDate = Monday,
                StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 30), Status = "Pending",
                User    = new User { Id = 5, FirstName = "Z", LastName = "Q", Email = "z@q.com", Role = "Client" },
                Service = new Service { Id = 1, Name = "Manicure", Description = string.Empty, Price = 20m, DurationMinutes = 45, IsActive = true }
            }
        };
        _repository.GetByUserId(5, Arg.Any<CancellationToken>()).Returns(appointments);

        // Act
        var result = (await _sut.GetByUserId(5)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(5, result[0].UserId);
    }

    [Fact]
    public async Task Delete_DelegatesToRepository()
    {
        // Arrange
        _repository.Delete(7, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Delete(7);

        // Assert
        Assert.True(result);
        await _repository.Received(1).Delete(7, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((Appointment?)null);

        // Act
        var result = await _sut.Update(99, new CreateAppointmentDto { ServiceId = 1 });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Update_WhenServiceNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange — no service seeded in context
        var existing = new Appointment
        {
            Id = 1, UserId = 1, ServiceId = 999, AppointmentDate = Monday,
            StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30), Status = "Pending",
            User    = new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", Role = "Client" },
            Service = new Service { Id = 999, Name = "X", Description = string.Empty, Price = 0m, DurationMinutes = 30, IsActive = true }
        };
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(existing);

        var dto = new CreateAppointmentDto { UserId = 1, ServiceId = 999, AppointmentDate = Monday, StartTime = new TimeOnly(10, 0) };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.Update(1, dto));
    }

    [Fact]
    public async Task Update_WhenValid_ReturnsUpdatedDto()
    {
        // Arrange
        await SeedBaseDataAsync();

        var existing = new Appointment
        {
            Id = 1, UserId = 1, ServiceId = 1, AppointmentDate = Monday,
            StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 30), Status = "Pending",
            User    = new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", Role = "Client" },
            Service = new Service { Id = 1, Name = "Haircut", Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true }
        };
        var updated = new Appointment
        {
            Id = 1, UserId = 1, ServiceId = 1, AppointmentDate = Monday,
            StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 30), Status = "Pending",
            User    = existing.User,
            Service = existing.Service
        };

        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(existing, updated);
        _repository.Update(Arg.Any<Appointment>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<Appointment>());

        var dto = new CreateAppointmentDto { UserId = 1, ServiceId = 1, AppointmentDate = Monday, StartTime = new TimeOnly(11, 0) };

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new TimeOnly(11, 30), result!.EndTime);
    }

    [Fact]
    public async Task Create_WhenServiceNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange — no service seeded
        var dto = new CreateAppointmentDto { UserId = 1, ServiceId = 999, AppointmentDate = Monday, StartTime = new TimeOnly(10, 0) };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task Create_WhenOutsideWorkingHours_ThrowsInvalidOperationException()
    {
        // Arrange — working hours 9-18, appointment starts at 08:00
        await SeedBaseDataAsync();

        var dto = new CreateAppointmentDto { UserId = 1, ServiceId = 1, AppointmentDate = Monday, StartTime = new TimeOnly(8, 0) };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Create(dto));
    }

    public void Dispose() => _context.Dispose();
}
