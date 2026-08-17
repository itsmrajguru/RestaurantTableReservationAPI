using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Data;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;

namespace RestaurantTableReservationAPI.Repositories;

public class RestaurantConfigRepository : IRestaurantConfigRepository
{
    private readonly AppDbContext _context;

    public RestaurantConfigRepository(AppDbContext context)
    {
        _context=context;
    }

    public async Task<RestaurantConfiguration?> GetConfigurationAsync()
    {
        return await _context.RestaurantConfigurations.FirstOrDefaultAsync();
    }

    public async Task UpdateConfigurationAsync(RestaurantConfiguration config)
    {
        _context.RestaurantConfigurations.Update(config);
        await _context.SaveChangesAsync();
    }
}
