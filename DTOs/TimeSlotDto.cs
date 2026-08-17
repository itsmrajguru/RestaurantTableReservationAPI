namespace RestaurantTableReservationAPI.DTOs;

public class TimeSlotResponseDto
{
    public int Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
}
