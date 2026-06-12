using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IWorkingHoursService
{
    Task<IEnumerable<WorkingHoursDto>> GetAll(CancellationToken ct = default);
    Task<WorkingHoursDto?> GetById(int id, CancellationToken ct = default);
    Task<WorkingHoursDto?> GetByDayOfWeek(DayOfWeek dayOfWeek, CancellationToken ct = default);
    Task<WorkingHoursDto> Create(CreateWorkingHoursDto dto, CancellationToken ct = default);
    Task<WorkingHoursDto?> Update(int id, CreateWorkingHoursDto dto, CancellationToken ct = default);
}
