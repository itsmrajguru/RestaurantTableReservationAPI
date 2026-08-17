using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class CreateReservationDto
{
    [Required]
    public DateOnly ReservationDate { get; set; }

    [Required]
    public int TimeSlotId { get; set; }

    [Required]
    [Range(1, 50, ErrorMessage = "Party size must be between 1 and 50.")]
    public int PartySize { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class ReservationResponseDto
{
    public int Id { get; set; }
    public DateOnly ReservationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PartySize { get; set; }
    public string? Notes { get; set; }
    
    public string TableNumber { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    
    // User info (useful for Admin views)
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
}
