using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService=reservationService;
    }

    [HttpPost]
    [Authorize] // Both Customers and Admins can create reservations
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
    {
        try
        {
            var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new{message="Invalid user token."});
            }

            var result=await _reservationService.CreateReservationAsync(userId, dto);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }

    [HttpGet("my")]
    [Authorize(Roles="Customer")]
    public async Task<IActionResult> GetMyReservations()
    {
        var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new{message="Invalid user token."});
        }

        var result=await _reservationService.GetCustomerReservationsAsync(userId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> GetAllReservations()
    {
        var result=await _reservationService.GetAllReservationsAsync();
        return Ok(result);
    }
}
