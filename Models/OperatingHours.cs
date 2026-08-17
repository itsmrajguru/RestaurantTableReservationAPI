using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.Models;

public class OperatingHours
{
    public int Id { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }

    public bool IsClosed { get; set; } = false;
}
