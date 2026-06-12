using AutoMapper;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BeautySaloon_API.Services;

public class SalonServiceService(
    IServiceRepository repository,
    IMemoryCache cache,
    IMapper mapper) : ISalonServiceService
{
    private const string CacheKey = "services:all:v1";

    private static readonly MemoryCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
        SlidingExpiration = TimeSpan.FromSeconds(15)
    };

    public async Task<IEnumerable<ServiceDto>> GetAll(CancellationToken ct = default)
    {
        if (cache.TryGetValue(CacheKey, out IEnumerable<ServiceDto>? cached) && cached is not null)
            return cached;

        var services = await repository.GetAll(ct);
        var result = mapper.Map<List<ServiceDto>>(services.Where(s => s.IsActive));

        cache.Set(CacheKey, result, CacheOptions);
        return result;
    }

    public async Task<ServiceDto?> GetById(int id, CancellationToken ct = default)
    {
        var service = await repository.GetById(id, ct);
        return service is null ? null : mapper.Map<ServiceDto>(service);
    }

    public async Task<ServiceDto> Create(CreateServiceDto dto, CancellationToken ct = default)
    {
        var service = mapper.Map<Models.Service>(dto);
        var created = mapper.Map<ServiceDto>(await repository.Create(service, ct));
        cache.Remove(CacheKey);
        return created;
    }

    public async Task<ServiceDto?> Update(int id, CreateServiceDto dto, CancellationToken ct = default)
    {
        var existing = await repository.GetById(id, ct);
        if (existing is null) return null;

        mapper.Map(dto, existing);
        var updated = mapper.Map<ServiceDto>(await repository.Update(existing, ct));
        cache.Remove(CacheKey);
        return updated;
    }

    public async Task<bool> Delete(int id, CancellationToken ct = default)
    {
        var deleted = await repository.Delete(id, ct);
        if (deleted) cache.Remove(CacheKey);
        return deleted;
    }
}
