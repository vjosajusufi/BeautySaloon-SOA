using BeautySaloon_API.Models;

namespace BeautySaloon_API.Repositories.Interfaces;

public interface IWorkingHoursRepository
{
    Task<IEnumerable<WorkingHours>> GetAll();
    Task<WorkingHours?> GetById(int id);
    Task<WorkingHours?> GetByDayOfWeek(DayOfWeek dayOfWeek);
    Task<WorkingHours> Create(WorkingHours workingHours);
    Task<WorkingHours> Update(WorkingHours workingHours);
}
