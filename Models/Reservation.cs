using System.ComponentModel.DataAnnotations;
using RestaurantTableReservationAPI.Models.Enums;

namespace RestaurantTableReservationAPI.Models;

public class Reservation
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int TableId { get; set; }
    public int TimeSlotId { get; set; }

    [Required]
    public DateOnly ReservationDate { get; set; }

    [Range(1, 20)]
    public int PartySize { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public RestaurantTable Table { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
}
