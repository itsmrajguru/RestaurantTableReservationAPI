using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Data;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Models.Enums;
using RestaurantTableReservationAPI.Repositories.Interfaces;

namespace RestaurantTableReservationAPI.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Reservation>> GetByUserIdAsync(int userId)
    {
        return await _context.Reservations
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReservationDate)
            .ThenBy(r => r.TimeSlot.StartTime)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetByDateAsync(DateOnly date)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .Where(r => r.ReservationDate == date)
            .OrderBy(r => r.TimeSlot.StartTime)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetAllAsync()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .OrderByDescending(r => r.ReservationDate)
            .ThenBy(r => r.TimeSlot.StartTime)
            .ToListAsync();
    }

    public async Task AddAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        reservation.UpdatedAt = DateTime.UtcNow;
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsTableBookedAsync(int tableId, DateOnly date, int timeSlotId)
    {
        // A table is booked if there's a Confirmed or Pending reservation for that table, date, and time slot.
        // Cancelled or NoShow means the table is free.
        return await _context.Reservations.AnyAsync(r =>
            r.TableId == tableId &&
            r.ReservationDate == date &&
            r.TimeSlotId == timeSlotId &&
            (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed));
    }
}
