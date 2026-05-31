using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface ISalonServiceService
{
    Task<IEnumerable<ServiceDto>> GetAll();
    Task<ServiceDto?> GetById(int id);
    Task<ServiceDto> Create(CreateServiceDto dto);
    Task<ServiceDto?> Update(int id, CreateServiceDto dto);
    Task<bool> Delete(int id);
}
