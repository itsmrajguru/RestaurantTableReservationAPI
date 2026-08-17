using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required!")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class RegisterCustomerDto
{
    [Required(ErrorMessage = "Name is required!")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters!")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required!")]
    [MinLength(4, ErrorMessage = "Password must be at least 4 characters long!")]
    public string Password { get; set; } = string.Empty;
}
