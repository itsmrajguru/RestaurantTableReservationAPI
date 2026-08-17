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

    public async Task<List<RestaurantTable>> GetAllAsync(bool includeInactive=false)
    {
        var query=_context.Tables.AsQueryable();
        if(!includeInactive)
        {
            query=query.Where(t=>t.IsActive);
        }
        return await query.ToListAsync();
    }

    public async Task<RestaurantTable?> GetByIdAsync(int id, bool includeInactive=false)
    {
        var query=_context.Tables.AsQueryable();
        if(!includeInactive)
        {
            query=query.Where(t=>t.IsActive);
        }
        return await query.FirstOrDefaultAsync(t=>t.Id==id);
    }

    public async Task<RestaurantTable> AddAsync(RestaurantTable table)
    {
        await _context.Tables.AddAsync(table);
        await _context.SaveChangesAsync();
        return table;
    }

    public async Task UpdateAsync(RestaurantTable table)
    {
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }
}
