using System.ComponentModel.DataAnnotations;
using RestaurantTableReservationAPI.Models.Enums;

namespace RestaurantTableReservationAPI.DTOs;

public class UpdateReservationStatusDto
{
    [Required]
    public ReservationStatus Status { get; set; }
}
