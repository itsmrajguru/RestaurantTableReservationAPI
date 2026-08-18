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

    /// <param name="id">1->09:00-10:00, 2->10:00-11:00, etc.</param>
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
        try
        {
            var createdSlot=await _timeSlotService.CreateTimeSlotAsync(dto);
            return CreatedAtAction(nameof(GetTimeSlotById), new{id=createdSlot!.Id}, createdSlot);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }

    /// <param name="id">1->09:00-10:00, 2->10:00-11:00, etc.</param>
    [HttpPut("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> UpdateTimeSlot(int id, UpdateTimeSlotDto dto)
    {
        try
        {
            var updatedSlot=await _timeSlotService.UpdateTimeSlotAsync(id, dto);
            if(updatedSlot==null) return NotFound(new{message="Time slot not found."});
            return Ok(updatedSlot);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }

    /// <param name="id">1->09:00-10:00, 2->10:00-11:00, etc.</param>
    [HttpDelete("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> DeleteTimeSlot(int id)
    {
        bool success=await _timeSlotService.DeleteTimeSlotAsync(id);
        if(!success) return NotFound(new{message="Time slot not found."});
        
        return NoContent();
    }
}
