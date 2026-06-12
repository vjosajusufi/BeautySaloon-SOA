using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class WorkingHoursRepository(AppDbContext context) : IWorkingHoursRepository
{
    public async Task<IEnumerable<WorkingHours>> GetAll(CancellationToken ct = default) =>
        await context.WorkingHours
            .AsNoTracking()
            .OrderBy(w => w.DayOfWeek)
            .ToListAsync(ct);

    public async Task<WorkingHours?> GetById(int id, CancellationToken ct = default) =>
        await context.WorkingHours.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<WorkingHours?> GetByDayOfWeek(DayOfWeek dayOfWeek, CancellationToken ct = default) =>
        await context.WorkingHours.AsNoTracking().FirstOrDefaultAsync(w => w.DayOfWeek == dayOfWeek, ct);

    public async Task<WorkingHours> Create(WorkingHours workingHours, CancellationToken ct = default)
    {
        context.WorkingHours.Add(workingHours);
        await context.SaveChangesAsync(ct);
        return workingHours;
    }

    public async Task<WorkingHours> Update(WorkingHours workingHours, CancellationToken ct = default)
    {
        context.Entry(workingHours).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
        return workingHours;
    }
}
