using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Data;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;

namespace RestaurantTableReservationAPI.Repositories;

public class TableRepository : ITableRepository
{
    private readonly AppDbContext _context;

    public TableRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RestaurantTable>> GetAllAsync()
    {
        return await _context.Tables.Where(t => t.IsActive).ToListAsync();
    }

    public async Task<RestaurantTable?> GetByIdAsync(int id)
    {
        return await _context.Tables.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
    }
}
