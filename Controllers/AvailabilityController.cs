using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService=availabilityService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailability([FromQuery] DateOnly date, [FromQuery] int partySize)
    {
        try
        {
            var result=await _availabilityService.GetAvailabilityAsync(date, partySize);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }
}
