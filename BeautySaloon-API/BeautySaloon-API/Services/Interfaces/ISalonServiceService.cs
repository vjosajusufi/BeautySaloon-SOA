using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface ISalonServiceService
{
    Task<IEnumerable<ServiceDto>> GetAll(CancellationToken ct = default);
    Task<ServiceDto?> GetById(int id, CancellationToken ct = default);
    Task<ServiceDto> Create(CreateServiceDto dto, CancellationToken ct = default);
    Task<ServiceDto?> Update(int id, CreateServiceDto dto, CancellationToken ct = default);
    Task<bool> Delete(int id, CancellationToken ct = default);
}
