using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAll();
    Task<AppointmentDto?> GetById(int id);
    Task<IEnumerable<AppointmentDto>> GetByUserId(int userId);
    Task<AppointmentDto> Create(CreateAppointmentDto dto);
    Task<AppointmentDto?> Update(int id, CreateAppointmentDto dto);
    Task<bool> Delete(int id);
}
