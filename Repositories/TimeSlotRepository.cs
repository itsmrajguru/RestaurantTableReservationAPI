using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Data;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;

namespace RestaurantTableReservationAPI.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly AppDbContext _context;

    public TimeSlotRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeSlot>> GetAllAsync()
    {
        return await _context.TimeSlots.Where(ts => ts.IsActive).OrderBy(ts => ts.StartTime).ToListAsync();
    }

    public async Task<TimeSlot?> GetByIdAsync(int id)
    {
        return await _context.TimeSlots.FirstOrDefaultAsync(ts => ts.Id == id && ts.IsActive);
    }
}
