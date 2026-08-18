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

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOperatingHoursById(int id)
    {
        var hours=await _operatingHoursService.GetOperatingHoursByIdAsync(id);
        if(hours==null) return NotFound(new{message="Operating hours not found."});
        return Ok(hours);
    }

    [HttpPut("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> UpdateOperatingHours(int id, [FromBody] UpdateOperatingHoursDto updateDto)
    {
        var updatedHours=await _operatingHoursService.UpdateOperatingHoursAsync(id, updateDto);
        if(updatedHours==null) return NotFound(new{message="Operating hours not found."});
        return Ok(updatedHours);
    }
}
