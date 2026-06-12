using System.Security.Claims;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautySaloon_API.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var appointments = await appointmentService.GetAll(ct);
        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var appointment = await appointmentService.GetById(id, ct);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy(CancellationToken ct = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var appointments = await appointmentService.GetByUserId(userId, ct);
        return Ok(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto, CancellationToken ct = default)
    {
        try
        {
            var created = await appointmentService.Create(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateAppointmentDto dto, CancellationToken ct = default)
    {
        try
        {
            var updated = await appointmentService.Update(id, dto, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var deleted = await appointmentService.Delete(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
