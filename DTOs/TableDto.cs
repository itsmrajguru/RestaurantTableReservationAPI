using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class TableResponseDto
{
    public int Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class CreateTableDto
{
    [Required(ErrorMessage = "Table number is required.")]
    [MaxLength(10, ErrorMessage = "Table number cannot exceed 10 characters.")]
    public string TableNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 20, ErrorMessage = "Table capacity must be between 1 and 20 people.")]
    public int Capacity { get; set; }

    [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    public string? Description { get; set; }
}
