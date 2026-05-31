using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IWorkingHoursService
{
    Task<IEnumerable<WorkingHoursDto>> GetAll();
    Task<WorkingHoursDto?> GetById(int id);
    Task<WorkingHoursDto?> GetByDayOfWeek(DayOfWeek dayOfWeek);
    Task<WorkingHoursDto> Create(CreateWorkingHoursDto dto);
    Task<WorkingHoursDto?> Update(int id, CreateWorkingHoursDto dto);
}
