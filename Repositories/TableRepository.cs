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


    /* this function is returning the data in the list format
    and again includeInactive=false is the default case*/
    public async Task<List<RestaurantTable>> GetAllAsync(bool includeInactive=false)
    {
        var query=_context.Tables.AsQueryable();
        //AsQueryable()-> create a blank request through which we'll request filtered data 
        if(!includeInactive)
        {
            query=query.Where(t=>t.IsActive);
        }
        return await query.ToListAsync();
        //ToListAsync()-->returns list
    }

    public async Task<RestaurantTable?> GetByIdAsync(int id, bool includeInactive=false)
    {
        var query=_context.Tables.AsQueryable();
        /* if the user is not admin, show him only the active tables */
        if(!includeInactive)
        {
            query=query.Where(t=>t.IsActive);
        }
        return await query.FirstOrDefaultAsync(t=>t.Id==id);
        /* 
        FirstOrDefaultAsync--> Sirf pehla wala le leta hai, error nahi
        SingleOrDefaultAsync--> null return karta hai	Error deta hai (expect karta hai sirf ek hi match ho)
        FirstAsync--> Pehla wala le leta hai */
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
