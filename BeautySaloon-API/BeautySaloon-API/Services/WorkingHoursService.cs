using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;

namespace BeautySaloon_API.Services;

public class WorkingHoursService(IWorkingHoursRepository repository) : IWorkingHoursService
{
    public async Task<IEnumerable<WorkingHoursDto>> GetAll()
    {
        var all = await repository.GetAll();
        return all.Select(ToDto);
    }

    public async Task<WorkingHoursDto?> GetById(int id)
    {
        var wh = await repository.GetById(id);
        return wh is null ? null : ToDto(wh);
    }

    public async Task<WorkingHoursDto?> GetByDayOfWeek(DayOfWeek dayOfWeek)
    {
        var wh = await repository.GetByDayOfWeek(dayOfWeek);
        return wh is null ? null : ToDto(wh);
    }

    public async Task<WorkingHoursDto> Create(CreateWorkingHoursDto dto)
    {
        var existing = await repository.GetByDayOfWeek(dto.DayOfWeek);
        if (existing is not null)
            throw new InvalidOperationException($"Working hours for {dto.DayOfWeek} already exist.");

        var wh = new WorkingHours
        {
            DayOfWeek = dto.DayOfWeek,
            OpenTime = dto.OpenTime,
            CloseTime = dto.CloseTime,
            IsOpen = dto.IsOpen
        };

        return ToDto(await repository.Create(wh));
    }

    public async Task<WorkingHoursDto?> Update(int id, CreateWorkingHoursDto dto)
    {
        var existing = await repository.GetById(id);
        if (existing is null) return null;

        existing.DayOfWeek = dto.DayOfWeek;
        existing.OpenTime = dto.OpenTime;
        existing.CloseTime = dto.CloseTime;
        existing.IsOpen = dto.IsOpen;

        return ToDto(await repository.Update(existing));
    }

    private static WorkingHoursDto ToDto(WorkingHours w) => new()
    {
        Id = w.Id,
        DayOfWeek = w.DayOfWeek,
        OpenTime = w.OpenTime,
        CloseTime = w.CloseTime,
        IsOpen = w.IsOpen
    };
}
