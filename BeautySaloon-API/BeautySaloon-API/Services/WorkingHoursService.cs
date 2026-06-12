using AutoMapper;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BeautySaloon_API.Services;

public class WorkingHoursService(
    IWorkingHoursRepository repository,
    IMemoryCache cache,
    IMapper mapper) : IWorkingHoursService
{
    private const string CacheKey = "workinghours:all:v1";

    private static readonly MemoryCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
        SlidingExpiration = TimeSpan.FromSeconds(15)
    };

    public async Task<IEnumerable<WorkingHoursDto>> GetAll(CancellationToken ct = default)
    {
        if (cache.TryGetValue(CacheKey, out IEnumerable<WorkingHoursDto>? cached) && cached is not null)
            return cached;

        var all = await repository.GetAll(ct);
        var result = mapper.Map<List<WorkingHoursDto>>(all);

        cache.Set(CacheKey, result, CacheOptions);
        return result;
    }

    public async Task<WorkingHoursDto?> GetById(int id, CancellationToken ct = default)
    {
        var wh = await repository.GetById(id, ct);
        return wh is null ? null : mapper.Map<WorkingHoursDto>(wh);
    }

    public async Task<WorkingHoursDto?> GetByDayOfWeek(DayOfWeek dayOfWeek, CancellationToken ct = default)
    {
        var wh = await repository.GetByDayOfWeek(dayOfWeek, ct);
        return wh is null ? null : mapper.Map<WorkingHoursDto>(wh);
    }

    public async Task<WorkingHoursDto> Create(CreateWorkingHoursDto dto, CancellationToken ct = default)
    {
        var existing = await repository.GetByDayOfWeek(dto.DayOfWeek, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Working hours for {dto.DayOfWeek} already exist.");

        var wh = mapper.Map<WorkingHours>(dto);
        var created = mapper.Map<WorkingHoursDto>(await repository.Create(wh, ct));
        cache.Remove(CacheKey);
        return created;
    }

    public async Task<WorkingHoursDto?> Update(int id, CreateWorkingHoursDto dto, CancellationToken ct = default)
    {
        var existing = await repository.GetById(id, ct);
        if (existing is null) return null;

        mapper.Map(dto, existing);
        var updated = mapper.Map<WorkingHoursDto>(await repository.Update(existing, ct));
        cache.Remove(CacheKey);
        return updated;
    }
}
