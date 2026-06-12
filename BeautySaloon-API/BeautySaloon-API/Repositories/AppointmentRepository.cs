using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
{
    public async Task<IEnumerable<Appointment>> GetAll(CancellationToken ct = default) =>
        await context.Appointments
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Service)
            .ToListAsync(ct);

    public async Task<Appointment?> GetById(int id, CancellationToken ct = default) =>
        await context.Appointments
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IEnumerable<Appointment>> GetByUserId(int userId, CancellationToken ct = default) =>
        await context.Appointments
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Service)
            .Where(a => a.UserId == userId)
            .ToListAsync(ct);

    public async Task<Appointment> Create(Appointment appointment, CancellationToken ct = default)
    {
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync(ct);
        return appointment;
    }

    public async Task<Appointment> Update(Appointment appointment, CancellationToken ct = default)
    {
        // Use Entry to avoid walking navigation properties loaded via AsNoTracking reads
        context.Entry(appointment).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
        return appointment;
    }

    public async Task<bool> Delete(int id, CancellationToken ct = default)
    {
        var appointment = await context.Appointments.FindAsync(new object[] { id }, ct);
        if (appointment is null) return false;
        context.Appointments.Remove(appointment);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
