using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.Models;

public class RestaurantConfiguration
{
    public int Id { get; set; }

    [Required]
    [Range(1, 50)]
    public int MaxPartySize { get; set; } = 20;

    [Required]
    [Range(0, 168)] // Max 1 week
    public int CancellationWindowHours { get; set; } = 2;

    [Required]
    [Range(1, 365)]
    public int AdvanceBookingDays { get; set; } = 30;
}
