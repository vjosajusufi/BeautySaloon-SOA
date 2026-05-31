using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautySaloon_API.Controllers;

[ApiController]
[Route("api/workinghours")]
public class WorkingHoursController(IWorkingHoursService workingHoursService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var workingHours = await workingHoursService.GetAll();
        return Ok(workingHours);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var workingHours = await workingHoursService.GetById(id);
        return workingHours is null ? NotFound() : Ok(workingHours);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateWorkingHoursDto dto)
    {
        try
        {
            var created = await workingHoursService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateWorkingHoursDto dto)
    {
        var updated = await workingHoursService.Update(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }
}
