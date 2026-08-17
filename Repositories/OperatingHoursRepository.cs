using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Data;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;

namespace RestaurantTableReservationAPI.Repositories;

public class OperatingHoursRepository : IOperatingHoursRepository
{
    private readonly AppDbContext _context;

    public OperatingHoursRepository(AppDbContext context)
    {
        _context=context;
    }

    public async Task<List<OperatingHours>> GetAllAsync()
    {
        return await _context.OperatingHours
            .OrderBy(o => o.DayOfWeek)
            .ToListAsync();
    }

    public async Task<OperatingHours?> GetByIdAsync(int id)
    {
        return await _context.OperatingHours.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<OperatingHours?> GetByDayAsync(DayOfWeek day)
    {
        return await _context.OperatingHours.FirstOrDefaultAsync(o => o.DayOfWeek == day);
    }

    public async Task UpdateAsync(OperatingHours operatingHours)
    {
        _context.OperatingHours.Update(operatingHours);
        await _context.SaveChangesAsync();
    }
}
