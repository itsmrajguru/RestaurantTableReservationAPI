using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Repositories.Interfaces;

public interface ITimeSlotRepository
{
    Task<List<TimeSlot>> GetAllAsync();
    Task<TimeSlot?> GetByIdAsync(int id);
}
