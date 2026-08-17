using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class RestaurantConfigResponseDto
{
    public int MaxPartySize { get; set; }
    public int CancellationWindowHours { get; set; }
    public int AdvanceBookingDays { get; set; }
}

public class UpdateRestaurantConfigDto
{
    [Required]
    [Range(1, 50, ErrorMessage = "Party size limit must be between 1 and 50.")]
    public int MaxPartySize { get; set; }

    [Required]
    [Range(0, 168, ErrorMessage = "Cancellation window must be between 0 and 168 hours.")]
    public int CancellationWindowHours { get; set; }

    [Required]
    [Range(1, 365, ErrorMessage = "Advance booking days must be between 1 and 365.")]
    public int AdvanceBookingDays { get; set; }
}
