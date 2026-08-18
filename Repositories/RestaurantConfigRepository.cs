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
        /* this means that simply look into database for RestaurantConfigurations table
        and return the data from it  */
        return await _context.RestaurantConfigurations.FirstOrDefaultAsync();
    }

    public async Task UpdateConfigurationAsync(RestaurantConfiguration config)
    {
        /* This means that simply take the new config and update into the table
        RestaurantConfigurations of the database and later save it */
        _context.RestaurantConfigurations.Update(config);
        await _context.SaveChangesAsync();
    }
}
