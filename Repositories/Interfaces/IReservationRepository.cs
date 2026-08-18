using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Repositories.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int id);
    Task<List<Reservation>> GetByUserIdAsync(int userId);
    Task<List<Reservation>> GetByDateAsync(DateOnly date);
    Task<List<Reservation>> GetAllAsync();
    Task<Reservation> CreateReservationWithConcurrencyCheckAsync(Reservation reservation);
    Task AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task<bool> IsTableBookedAsync(int tableId, DateOnly date, int timeSlotId);
}
