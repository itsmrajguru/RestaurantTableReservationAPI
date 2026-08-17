using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class OperatingHoursResponseDto
{
    public int Id { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
    public bool IsClosed { get; set; }
}

public class UpdateOperatingHoursDto
{
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
    
    [Required]
    public bool IsClosed { get; set; }
}
