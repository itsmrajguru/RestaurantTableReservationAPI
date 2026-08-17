using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.Models;

public class TimeSlot
{
    public int Id { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
