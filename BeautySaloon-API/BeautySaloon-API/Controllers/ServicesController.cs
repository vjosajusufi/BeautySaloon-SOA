using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BeautySaloon_API.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController(ISalonServiceService salonServiceService) : ControllerBase
{
    [HttpGet]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var services = await salonServiceService.GetAll(ct);
        return Ok(services);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var service = await salonServiceService.GetById(id, ct);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateServiceDto dto, CancellationToken ct = default)
    {
        var created = await salonServiceService.Create(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateServiceDto dto, CancellationToken ct = default)
    {
        var updated = await salonServiceService.Update(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var deleted = await salonServiceService.Delete(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
