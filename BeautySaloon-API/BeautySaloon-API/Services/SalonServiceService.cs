using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;

namespace BeautySaloon_API.Services;

public class SalonServiceService(IServiceRepository repository) : ISalonServiceService
{
    public async Task<IEnumerable<ServiceDto>> GetAll()
    {
        var services = await repository.GetAll();
        return services.Where(s => s.IsActive).Select(ToDto);
    }

    public async Task<ServiceDto?> GetById(int id)
    {
        var service = await repository.GetById(id);
        return service is null ? null : ToDto(service);
    }

    public async Task<ServiceDto> Create(CreateServiceDto dto)
    {
        var service = new Service
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DurationMinutes = dto.DurationMinutes,
            IsActive = true
        };
        return ToDto(await repository.Create(service));
    }

    public async Task<ServiceDto?> Update(int id, CreateServiceDto dto)
    {
        var existing = await repository.GetById(id);
        if (existing is null) return null;

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Price = dto.Price;
        existing.DurationMinutes = dto.DurationMinutes;

        return ToDto(await repository.Update(existing));
    }

    public async Task<bool> Delete(int id) =>
        await repository.Delete(id);

    private static ServiceDto ToDto(Service s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
        Price = s.Price,
        DurationMinutes = s.DurationMinutes,
        IsActive = s.IsActive
    };
}
