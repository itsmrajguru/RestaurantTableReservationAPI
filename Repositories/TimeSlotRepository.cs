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

    public async Task<List<TimeSlot>> GetAllAsync(bool includeInactive=false)
    {
        var query=_context.TimeSlots.AsQueryable();
        if(!includeInactive)
        {
            query=query.Where(ts=>ts.IsActive);
        }
        return await query.OrderBy(ts=>ts.StartTime).ToListAsync();
    }

    public async Task<TimeSlot?> GetByIdAsync(int id, bool includeInactive=false)
    {
        var query=_context.TimeSlots.AsQueryable();
        if(!includeInactive)
        {
            query=query.Where(ts=>ts.IsActive);
        }
        return await query.FirstOrDefaultAsync(ts=>ts.Id==id);
    }

    public async Task<TimeSlot> AddAsync(TimeSlot timeSlot)
    {
        await _context.TimeSlots.AddAsync(timeSlot);
        await _context.SaveChangesAsync();
        return timeSlot;
    }

    public async Task UpdateAsync(TimeSlot timeSlot)
    {
        _context.TimeSlots.Update(timeSlot);
        await _context.SaveChangesAsync();
    }
}
