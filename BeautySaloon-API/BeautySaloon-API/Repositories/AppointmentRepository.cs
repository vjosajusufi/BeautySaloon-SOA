using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
{
    public async Task<IEnumerable<Appointment>> GetAll() =>
        await context.Appointments
            .Include(a => a.User)
            .Include(a => a.Service)
            .ToListAsync();

    public async Task<Appointment?> GetById(int id) =>
        await context.Appointments
            .Include(a => a.User)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Appointment>> GetByUserId(int userId) =>
        await context.Appointments
            .Include(a => a.User)
            .Include(a => a.Service)
            .Where(a => a.UserId == userId)
            .ToListAsync();

    public async Task<Appointment> Create(Appointment appointment)
    {
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();
        return appointment;
    }

    public async Task<Appointment> Update(Appointment appointment)
    {
        context.Appointments.Update(appointment);
        await context.SaveChangesAsync();
        return appointment;
    }

    public async Task<bool> Delete(int id)
    {
        var appointment = await context.Appointments.FindAsync(id);
        if (appointment is null) return false;
        context.Appointments.Remove(appointment);
        await context.SaveChangesAsync();
        return true;
    }
}
