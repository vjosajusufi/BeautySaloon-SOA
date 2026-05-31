using AutoMapper;
using BeautySaloon_API.Data;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Services;

public class AppointmentService(
    IAppointmentRepository repository,
    AppDbContext context,
    IMapper mapper) : IAppointmentService
{
    public async Task<IEnumerable<AppointmentDto>> GetAll(CancellationToken ct = default)
    {
        var appointments = await repository.GetAll(ct);
        return mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }

    public async Task<AppointmentDto?> GetById(int id, CancellationToken ct = default)
    {
        var appointment = await repository.GetById(id, ct);
        return appointment is null ? null : mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByUserId(int userId, CancellationToken ct = default)
    {
        var appointments = await repository.GetByUserId(userId, ct);
        return mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }

    public async Task<AppointmentDto> Create(CreateAppointmentDto dto, CancellationToken ct = default)
    {
        var service = await context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == dto.ServiceId, ct)
            ?? throw new KeyNotFoundException("Service not found.");

        var endTime = dto.StartTime.AddMinutes(service.DurationMinutes);

        await ValidateWorkingHours(dto.AppointmentDate, dto.StartTime, endTime, ct);
        await ValidateNoConflict(dto.ServiceId, dto.AppointmentDate, dto.StartTime, endTime, excludeId: null, ct);

        var appointment = mapper.Map<Appointment>(dto);
        appointment.EndTime = endTime;

        var created = await repository.Create(appointment, ct);
        return mapper.Map<AppointmentDto>((await repository.GetById(created.Id, ct))!);
    }

    public async Task<AppointmentDto?> Update(int id, CreateAppointmentDto dto, CancellationToken ct = default)
    {
        var existing = await repository.GetById(id, ct);
        if (existing is null) return null;

        var service = await context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == dto.ServiceId, ct)
            ?? throw new KeyNotFoundException("Service not found.");

        var endTime = dto.StartTime.AddMinutes(service.DurationMinutes);

        await ValidateWorkingHours(dto.AppointmentDate, dto.StartTime, endTime, ct);
        await ValidateNoConflict(dto.ServiceId, dto.AppointmentDate, dto.StartTime, endTime, excludeId: id, ct);

        mapper.Map(dto, existing);
        existing.EndTime = endTime;

        await repository.Update(existing, ct);
        return mapper.Map<AppointmentDto>((await repository.GetById(id, ct))!);
    }

    public async Task<bool> Delete(int id, CancellationToken ct = default) =>
        await repository.Delete(id, ct);

    private async Task ValidateWorkingHours(
        DateOnly date, TimeOnly startTime, TimeOnly endTime, CancellationToken ct)
    {
        var workingHours = await context.WorkingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.DayOfWeek == date.DayOfWeek, ct);

        if (workingHours is null || !workingHours.IsOpen)
            throw new InvalidOperationException($"The salon is closed on {date.DayOfWeek}.");

        if (startTime < workingHours.OpenTime || endTime > workingHours.CloseTime)
            throw new InvalidOperationException(
                $"Appointment must be within working hours ({workingHours.OpenTime} – {workingHours.CloseTime}).");
    }

    private async Task ValidateNoConflict(
        int serviceId, DateOnly date, TimeOnly startTime, TimeOnly endTime, int? excludeId, CancellationToken ct)
    {
        var hasConflict = await context.Appointments
            .AsNoTracking()
            .Where(a => a.ServiceId == serviceId
                     && a.AppointmentDate == date
                     && a.Status != "Cancelled"
                     && (excludeId == null || a.Id != excludeId)
                     && a.StartTime < endTime
                     && a.EndTime > startTime)
            .AnyAsync(ct);

        if (hasConflict)
            throw new InvalidOperationException("This time slot is already booked for the selected service.");
    }
}
