using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class WorkingHoursRepository(AppDbContext context) : IWorkingHoursRepository
{
    public async Task<IEnumerable<WorkingHours>> GetAll() =>
        await context.WorkingHours
            .OrderBy(w => w.DayOfWeek)
            .ToListAsync();

    public async Task<WorkingHours?> GetById(int id) =>
        await context.WorkingHours.FindAsync(id);

    public async Task<WorkingHours?> GetByDayOfWeek(DayOfWeek dayOfWeek) =>
        await context.WorkingHours.FirstOrDefaultAsync(w => w.DayOfWeek == dayOfWeek);

    public async Task<WorkingHours> Create(WorkingHours workingHours)
    {
        context.WorkingHours.Add(workingHours);
        await context.SaveChangesAsync();
        return workingHours;
    }

    public async Task<WorkingHours> Update(WorkingHours workingHours)
    {
        context.WorkingHours.Update(workingHours);
        await context.SaveChangesAsync();
        return workingHours;
    }
}
