using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Repositories.Interfaces;

public interface IRestaurantConfigRepository
{
    Task<RestaurantConfiguration?> GetConfigurationAsync();
    Task UpdateConfigurationAsync(RestaurantConfiguration config);
}
