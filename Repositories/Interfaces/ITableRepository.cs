using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Repositories.Interfaces;

public interface ITableRepository
{
    Task<List<RestaurantTable>> GetAllAsync();
    Task<RestaurantTable?> GetByIdAsync(int id);
}
