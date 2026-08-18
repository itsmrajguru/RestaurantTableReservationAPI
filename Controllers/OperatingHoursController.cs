using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperatingHoursController : ControllerBase
{
    private readonly IOperatingHoursService _operatingHoursService;
    public OperatingHoursController(IOperatingHoursService operatingHoursService)
    {
        _operatingHoursService=operatingHoursService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllOperatingHours()
    {
        var hours=await _operatingHoursService.GetAllOperatingHoursAsync();
        return Ok(hours);
    }

    /// <param name="id">1->Sunday, 2->Monday, 3->Tuesday, 4->Wednesday, 5->Thursday, 6->Friday, 7->Saturday</param>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOperatingHoursById(int id)
    {
        var hours=await _operatingHoursService.GetOperatingHoursByIdAsync(id);
        if(hours==null) return NotFound(new{message="Operating hours not found."});
        return Ok(hours);
    }

    [HttpGet("day/{day}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOperatingHoursByDay(DayOfWeek day)
    {
        var hours = await _operatingHoursService.GetOperatingHoursByDayAsync(day);
        if(hours == null) return NotFound(new{message="Operating hours not found for this day."});
        return Ok(hours);
    }

    /// <param name="id">1->Sunday, 2->Monday, 3->Tuesday, 4->Wednesday, 5->Thursday, 6->Friday, 7->Saturday</param>
    [HttpPut("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> UpdateOperatingHours(int id, [FromBody] UpdateOperatingHoursDto updateDto)
    {
        var updatedHours=await _operatingHoursService.UpdateOperatingHoursAsync(id, updateDto);
        if(updatedHours==null) return NotFound(new{message="Operating hours not found."});
        return Ok(updatedHours);
    }
}
