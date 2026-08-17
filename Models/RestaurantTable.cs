using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.Models;

public class RestaurantTable
{
    public int Id { get; set; }

    [Required, MaxLength(10)]
    public string TableNumber { get; set; } = string.Empty;

    [Range(1, 50)]
    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
