using System.ComponentModel.DataAnnotations;
using RestaurantTableReservationAPI.Models.Enums;

namespace RestaurantTableReservationAPI.DTOs;

public class ReservationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public int TableId { get; set; }
    public string TableNumber { get; set; } = string.Empty;

    public int TimeSlotId { get; set; }
    public TimeSpan StartTime { get; set; }

    public DateOnly ReservationDate { get; set; }
    public int PartySize { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateReservationDto
{
    [Required(ErrorMessage = "TableId is required!")]
    public int TableId { get; set; }

    [Required(ErrorMessage = "TimeSlotId is required!")]
    public int TimeSlotId { get; set; }

    [Required(ErrorMessage = "Reservation date is required!")]
    public DateOnly ReservationDate { get; set; }

    [Required(ErrorMessage = "Party size is required!")]
    [Range(1, 20, ErrorMessage = "Party size must be between 1 and 20!")]
    public int PartySize { get; set; }

    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters!")]
    public string? Notes { get; set; }
}
