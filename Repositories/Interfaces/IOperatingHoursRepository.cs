using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Repositories.Interfaces;

public interface IOperatingHoursRepository
{
    Task<List<OperatingHours>> GetAllAsync();
    Task<OperatingHours?> GetByIdAsync(int id);
    Task<OperatingHours?> GetByDayAsync(DayOfWeek day);
    Task UpdateAsync(OperatingHours operatingHours);
}
