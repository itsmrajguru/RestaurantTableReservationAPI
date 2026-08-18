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
        var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        var result=await _reservationService.CreateReservationAsync(userId, dto);
        return StatusCode(201, result);
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

    /// <param name="id">1->Reservation 1, 2->Reservation 2, etc.</param>
    [HttpGet("{id}")]
    [Authorize(Roles="Customer")]
    public async Task<IActionResult> GetReservationDetails(int id)
    {
        var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        var result=await _reservationService.GetCustomerReservationByIdAsync(id, userId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> GetAllReservations()
    {
        var result=await _reservationService.GetAllReservationsAsync();
        return Ok(result);
    }

    /// <param name="id">1->Reservation 1, 2->Reservation 2, etc.</param>
    [HttpPut("{id}/confirm")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> ConfirmReservation(int id)
    {
        var result=await _reservationService.ConfirmReservationAsync(id);
        return Ok(result);
    }

    /// <param name="id">1->Reservation 1, 2->Reservation 2, etc.</param>
    [HttpPut("{id}/check-in")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> CheckInReservation(int id)
    {
        var result=await _reservationService.CheckInAsync(id);
        return Ok(result);
    }

    /// <param name="id">1->Reservation 1, 2->Reservation 2, etc.</param>
    [HttpPut("{id}/no-show")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> MarkNoShow(int id)
    {
        var result=await _reservationService.MarkNoShowAsync(id);
        return Ok(result);
    }

    /// <param name="id">1->Reservation 1, 2->Reservation 2, etc.</param>
    [HttpPut("{id}/complete")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> CompleteReservation(int id)
    {
        var result=await _reservationService.CompleteReservationAsync(id);
        return Ok(result);
    }

    [HttpPost("walk-in")]
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> HandleWalkIn([FromBody] CreateWalkInDto dto)
    {
        var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        var result=await _reservationService.HandleWalkInAsync(userId, dto);
        return Ok(result);
    }

    /// <param name="id">1->Reservation 1, 2->Reservation 2, etc.</param>
    [HttpPut("{id}/cancel")]
    [Authorize(Roles="Customer,Admin,Staff")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var userIdString=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("Staff");
        var result=await _reservationService.CancelReservationAsync(id, userId, isAdmin);
        return Ok(new{message="Reservation successfully cancelled."});
    }
}
