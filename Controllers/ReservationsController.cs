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
        catch(InvalidOperationException ex)
        {
            return Conflict(new{message=ex.Message});
        }
    }

    [HttpGet("my/upcoming")]
    [Authorize(Roles="Customer")]
    public async Task<IActionResult> GetMyUpcomingReservations()
    {
        var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new{message="Invalid user token."});
        }

        var result=await _reservationService.GetCustomerUpcomingReservationsAsync(userId);
        return Ok(result);
    }

    [HttpGet("my/history")]
    [Authorize(Roles="Customer")]
    public async Task<IActionResult> GetMyReservationHistory()
    {
        var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new{message="Invalid user token."});
        }

        var result=await _reservationService.GetCustomerReservationHistoryAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles="Customer")]
    public async Task<IActionResult> GetReservationDetails(int id)
    {
        try
        {
            var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new{message="Invalid user token."});
            }

            var result=await _reservationService.GetCustomerReservationByIdAsync(id, userId);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return NotFound(new{message=ex.Message});
        }
        catch(UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> GetAllReservations()
    {
        var result=await _reservationService.GetAllReservationsAsync();
        return Ok(result);
    }

    [HttpPut("{id}/confirm")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> ConfirmReservation(int id)
    {
        try
        {
            var result=await _reservationService.ConfirmReservationAsync(id);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }

    [HttpPut("{id}/check-in")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> CheckInReservation(int id)
    {
        try
        {
            var result=await _reservationService.CheckInAsync(id);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }

    [HttpPut("{id}/no-show")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> MarkNoShow(int id)
    {
        try
        {
            var result=await _reservationService.MarkNoShowAsync(id);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }

    [HttpPut("{id}/complete")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> CompleteReservation(int id)
    {
        try
        {
            var result=await _reservationService.CompleteReservationAsync(id);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }

    [HttpPost("walk-in")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> HandleWalkIn([FromBody] CreateWalkInDto dto)
    {
        try
        {
            var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new{message="Invalid user token."});
            }

            var result=await _reservationService.HandleWalkInAsync(userId, dto);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
        catch(InvalidOperationException ex)
        {
            return Conflict(new{message=ex.Message});
        }
    }

    [HttpPut("{id}/cancel")]
    [Authorize(Roles="Customer")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        try
        {
            var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new{message="Invalid user token."});
            }

            var result=await _reservationService.CancelReservationAsync(id, userId);
            return Ok(new{message="Reservation successfully cancelled."});
        }
        catch(UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new{message=ex.Message});
        }
    }
}
