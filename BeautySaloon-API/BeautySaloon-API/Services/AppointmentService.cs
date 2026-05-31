using BeautySaloon_API.Data;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Services;

public class AppointmentService(IAppointmentRepository repository, AppDbContext context) : IAppointmentService
{
    public async Task<IEnumerable<AppointmentDto>> GetAll()
    {
        var appointments = await repository.GetAll();
        return appointments.Select(ToDto);
    }

    public async Task<AppointmentDto?> GetById(int id)
    {
        var appointment = await repository.GetById(id);
        return appointment is null ? null : ToDto(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByUserId(int userId)
    {
        var appointments = await repository.GetByUserId(userId);
        return appointments.Select(ToDto);
    }

    public async Task<AppointmentDto> Create(CreateAppointmentDto dto)
    {
        var service = await context.Services.FindAsync(dto.ServiceId)
            ?? throw new KeyNotFoundException("Service not found.");

        var endTime = dto.StartTime.AddMinutes(service.DurationMinutes);

        await ValidateWorkingHours(dto.AppointmentDate, dto.StartTime, endTime);
        await ValidateNoConflict(dto.ServiceId, dto.AppointmentDate, dto.StartTime, endTime, excludeId: null);

        var appointment = new Appointment
        {
            UserId = dto.UserId,
            ServiceId = dto.ServiceId,
            AppointmentDate = dto.AppointmentDate,
            StartTime = dto.StartTime,
            EndTime = endTime,
            Status = "Pending",
            Notes = dto.Notes
        };

        var created = await repository.Create(appointment);
        return ToDto((await repository.GetById(created.Id))!);
    }

    public async Task<AppointmentDto?> Update(int id, CreateAppointmentDto dto)
    {
        var existing = await repository.GetById(id);
        if (existing is null) return null;

        var service = await context.Services.FindAsync(dto.ServiceId)
            ?? throw new KeyNotFoundException("Service not found.");

        var endTime = dto.StartTime.AddMinutes(service.DurationMinutes);

        await ValidateWorkingHours(dto.AppointmentDate, dto.StartTime, endTime);
        await ValidateNoConflict(dto.ServiceId, dto.AppointmentDate, dto.StartTime, endTime, excludeId: id);

        existing.UserId = dto.UserId;
        existing.ServiceId = dto.ServiceId;
        existing.AppointmentDate = dto.AppointmentDate;
        existing.StartTime = dto.StartTime;
        existing.EndTime = endTime;
        existing.Notes = dto.Notes;

        await repository.Update(existing);
        return ToDto((await repository.GetById(id))!);
    }

    public async Task<bool> Delete(int id) =>
        await repository.Delete(id);

    private async Task ValidateWorkingHours(DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        var workingHours = await context.WorkingHours
            .FirstOrDefaultAsync(w => w.DayOfWeek == date.DayOfWeek);

        if (workingHours is null || !workingHours.IsOpen)
            throw new InvalidOperationException($"The salon is closed on {date.DayOfWeek}.");

        if (startTime < workingHours.OpenTime || endTime > workingHours.CloseTime)
            throw new InvalidOperationException(
                $"Appointment must be within working hours ({workingHours.OpenTime} – {workingHours.CloseTime}).");
    }

    private async Task ValidateNoConflict(
        int serviceId, DateOnly date, TimeOnly startTime, TimeOnly endTime, int? excludeId)
    {
        var hasConflict = await context.Appointments
            .Where(a => a.ServiceId == serviceId
                     && a.AppointmentDate == date
                     && a.Status != "Cancelled"
                     && (excludeId == null || a.Id != excludeId)
                     && a.StartTime < endTime
                     && a.EndTime > startTime)
            .AnyAsync();

        if (hasConflict)
            throw new InvalidOperationException("This time slot is already booked for the selected service.");
    }

    private static AppointmentDto ToDto(Appointment a) => new()
    {
        Id = a.Id,
        UserId = a.UserId,
        UserFullName = $"{a.User.FirstName} {a.User.LastName}",
        ServiceId = a.ServiceId,
        ServiceName = a.Service.Name,
        AppointmentDate = a.AppointmentDate,
        StartTime = a.StartTime,
        EndTime = a.EndTime,
        Status = a.Status,
        Notes = a.Notes
    };
}
