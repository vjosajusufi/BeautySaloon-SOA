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

public class AppointmentServiceTests : IDisposable
{
    private readonly IAppointmentRepository _repository;
    private readonly AppDbContext _context;
    private readonly AppointmentService _sut;

    public AppointmentServiceTests()
    {
        _repository = Substitute.For<IAppointmentRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
            .CreateMapper();

        _sut = new AppointmentService(_repository, _context, mapper);
    }

    [Fact]
    public async Task BookAppointment_WhenSalonIsClosed_ThrowsException()
    {
        // Arrange — June 1 2025 is a Sunday; seed the salon as closed that day
        var date = new DateOnly(2025, 6, 1);

        _context.Services.Add(new Service
        {
            Id = 1, Name = "Haircut", Description = string.Empty,
            Price = 15m, DurationMinutes = 30, IsActive = true
        });
        _context.WorkingHours.Add(new WorkingHours
        {
            Id = 1, DayOfWeek = DayOfWeek.Sunday,
            OpenTime = new TimeOnly(0, 0), CloseTime = new TimeOnly(0, 0), IsOpen = false
        });
        await _context.SaveChangesAsync();

        var dto = new CreateAppointmentDto
        {
            UserId = 1, ServiceId = 1,
            AppointmentDate = date, StartTime = new TimeOnly(10, 0)
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task BookAppointment_WhenDoubleBooking_ThrowsException()
    {
        // Arrange — June 2 2025 is a Monday; seed an overlapping appointment
        var date = new DateOnly(2025, 6, 2);

        _context.Services.Add(new Service
        {
            Id = 1, Name = "Haircut", Description = string.Empty,
            Price = 15m, DurationMinutes = 30, IsActive = true
        });
        _context.WorkingHours.Add(new WorkingHours
        {
            Id = 1, DayOfWeek = DayOfWeek.Monday,
            OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0), IsOpen = true
        });
        _context.Appointments.Add(new Appointment
        {
            Id = 1, UserId = 2, ServiceId = 1,
            AppointmentDate = date,
            StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30),
            Status = "Pending"
        });
        await _context.SaveChangesAsync();

        var dto = new CreateAppointmentDto
        {
            UserId = 1, ServiceId = 1,
            AppointmentDate = date, StartTime = new TimeOnly(10, 0) // overlaps exactly
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task BookAppointment_WhenValidRequest_ReturnsAppointmentDto()
    {
        // Arrange — Monday, no conflicts, salon is open
        var date = new DateOnly(2025, 6, 2);

        _context.Services.Add(new Service
        {
            Id = 1, Name = "Haircut", Description = string.Empty,
            Price = 15m, DurationMinutes = 30, IsActive = true
        });
        _context.WorkingHours.Add(new WorkingHours
        {
            Id = 1, DayOfWeek = DayOfWeek.Monday,
            OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0), IsOpen = true
        });
        await _context.SaveChangesAsync();

        var savedAppointment = new Appointment
        {
            Id = 1, UserId = 1, ServiceId = 1, AppointmentDate = date,
            StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30),
            Status = "Pending", Notes = "First visit"
        };
        var appointmentWithNav = new Appointment
        {
            Id = 1, UserId = 1, ServiceId = 1, AppointmentDate = date,
            StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30),
            Status = "Pending", Notes = "First visit",
            User = new User { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", Role = "Client" },
            Service = new Service { Id = 1, Name = "Haircut", Description = string.Empty, Price = 15m, DurationMinutes = 30, IsActive = true }
        };

        _repository.Create(Arg.Any<Appointment>(), Arg.Any<CancellationToken>())
                   .Returns(savedAppointment);
        _repository.GetById(1, Arg.Any<CancellationToken>())
                   .Returns(appointmentWithNav);

        var dto = new CreateAppointmentDto
        {
            UserId = 1, ServiceId = 1,
            AppointmentDate = date, StartTime = new TimeOnly(10, 0), Notes = "First visit"
        };

        // Act
        var result = await _sut.Create(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Haircut", result.ServiceName);
        Assert.Equal("Jane Doe", result.UserFullName);
        Assert.Equal(new TimeOnly(10, 30), result.EndTime);
        Assert.Equal("Pending", result.Status);
    }

    public void Dispose() => _context.Dispose();
}
