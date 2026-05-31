using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautySaloon_API.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController(ISalonServiceService salonServiceService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var services = await salonServiceService.GetAll();
        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var service = await salonServiceService.GetById(id);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateServiceDto dto)
    {
        var created = await salonServiceService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateServiceDto dto)
    {
        var updated = await salonServiceService.Update(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await salonServiceService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}
