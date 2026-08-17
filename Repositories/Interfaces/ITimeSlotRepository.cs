using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Repositories.Interfaces;

public interface ITimeSlotRepository
{
    Task<List<TimeSlot>> GetAllAsync(bool includeInactive=false);
    Task<TimeSlot?> GetByIdAsync(int id, bool includeInactive=false);
    Task<TimeSlot> AddAsync(TimeSlot timeSlot);
    Task UpdateAsync(TimeSlot timeSlot);
}
