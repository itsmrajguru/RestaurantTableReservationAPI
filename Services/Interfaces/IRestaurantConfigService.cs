using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface IRestaurantConfigService
{
    Task<RestaurantConfigResponseDto?> GetConfigurationAsync();
    Task<RestaurantConfigResponseDto?> UpdateConfigurationAsync(UpdateRestaurantConfigDto updateDto);
}
