using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAll(CancellationToken ct = default);
    Task<AppointmentDto?> GetById(int id, CancellationToken ct = default);
    Task<IEnumerable<AppointmentDto>> GetByUserId(int userId, CancellationToken ct = default);
    Task<AppointmentDto> Create(CreateAppointmentDto dto, CancellationToken ct = default);
    Task<AppointmentDto?> Update(int id, CreateAppointmentDto dto, CancellationToken ct = default);
    Task<bool> Delete(int id, CancellationToken ct = default);
}
