using BeautySaloon_API.Models;

namespace BeautySaloon_API.Repositories.Interfaces;

public interface IWorkingHoursRepository
{
    Task<IEnumerable<WorkingHours>> GetAll(CancellationToken ct = default);
    Task<WorkingHours?> GetById(int id, CancellationToken ct = default);
    Task<WorkingHours?> GetByDayOfWeek(DayOfWeek dayOfWeek, CancellationToken ct = default);
    Task<WorkingHours> Create(WorkingHours workingHours, CancellationToken ct = default);
    Task<WorkingHours> Update(WorkingHours workingHours, CancellationToken ct = default);
}
