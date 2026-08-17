using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class AvailabilityRequestDto
{
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(1, 50, ErrorMessage = "Party size must be between 1 and 50.")]
    public int PartySize { get; set; }
}

public class AvailableTimeSlotDto
{
    public int TimeSlotId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

public class AvailabilityResponseDto
{
    public DateOnly Date { get; set; }
    public int PartySize { get; set; }
    public List<AvailableTimeSlotDto> AvailableTimeSlots { get; set; } = new List<AvailableTimeSlotDto>();
}
