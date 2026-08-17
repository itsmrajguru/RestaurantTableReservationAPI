using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotsController(ITimeSlotService timeSlotService)
    {
        _timeSlotService=timeSlotService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTimeSlots([FromQuery] bool includeInactive=false)
    {
        bool isAdmin=User.IsInRole("Admin");
        if(includeInactive && !isAdmin)
        {
            includeInactive=false;
        }

        var slots=await _timeSlotService.GetAllTimeSlotsAsync(includeInactive);
        return Ok(slots);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTimeSlotById(int id)
    {
        bool isAdmin=User.IsInRole("Admin");
        var slot=await _timeSlotService.GetTimeSlotByIdAsync(id, isAdmin);
        if(slot==null) return NotFound(new{message="Time slot not found."});
        return Ok(slot);
    }

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> CreateTimeSlot(CreateTimeSlotDto dto)
    {
        var createdSlot=await _timeSlotService.CreateTimeSlotAsync(dto);
        if(createdSlot==null) return BadRequest(new{message="Invalid time range. StartTime must be before EndTime."});
        
        return CreatedAtAction(nameof(GetTimeSlotById), new{id=createdSlot.Id}, createdSlot);
    }

    [HttpPut("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> UpdateTimeSlot(int id, UpdateTimeSlotDto dto)
    {
        var updatedSlot=await _timeSlotService.UpdateTimeSlotAsync(id, dto);
        if(updatedSlot==null) return BadRequest(new{message="Invalid update request. Verify ID and Time Range."});
        
        return Ok(updatedSlot);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> DeleteTimeSlot(int id)
    {
        bool success=await _timeSlotService.DeleteTimeSlotAsync(id);
        if(!success) return NotFound(new{message="Time slot not found."});
        
        return NoContent();
    }
}
