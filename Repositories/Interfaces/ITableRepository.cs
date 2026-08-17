using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Repositories.Interfaces;

public interface ITableRepository
{
    Task<List<RestaurantTable>> GetAllAsync(bool includeInactive=false);
    Task<RestaurantTable?> GetByIdAsync(int id, bool includeInactive=false);
    Task<RestaurantTable> AddAsync(RestaurantTable table);
    Task UpdateAsync(RestaurantTable table);
}
