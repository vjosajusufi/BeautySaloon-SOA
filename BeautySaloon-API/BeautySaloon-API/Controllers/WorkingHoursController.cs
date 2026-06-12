using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BeautySaloon_API.Controllers;

[ApiController]
[Route("api/workinghours")]
public class WorkingHoursController(IWorkingHoursService workingHoursService) : ControllerBase
{
    [HttpGet]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var workingHours = await workingHoursService.GetAll(ct);
        return Ok(workingHours);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var workingHours = await workingHoursService.GetById(id, ct);
        return workingHours is null ? NotFound() : Ok(workingHours);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateWorkingHoursDto dto, CancellationToken ct = default)
    {
        try
        {
            var created = await workingHoursService.Create(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateWorkingHoursDto dto, CancellationToken ct = default)
    {
        var updated = await workingHoursService.Update(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }
}
