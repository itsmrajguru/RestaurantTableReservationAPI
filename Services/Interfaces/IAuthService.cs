using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginDto);
    Task<bool> RegisterCustomerAsync(RegisterCustomerDto registerDto);
}
