using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class CreateWalkInDto
{
    [Required]
    [Range(1, 20)]
    public int PartySize { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
