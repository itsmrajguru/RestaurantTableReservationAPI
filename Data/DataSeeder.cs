using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {

        var users = new List<User>
        {
            new User
            {
                Name = "Admin User",
                Email = "admin@restaurant.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRoles.Admin,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Name = "Staff User",
                Email = "staff@restaurant.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                Role = UserRoles.Staff,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Name = "Customer User",
                Email = "customer@restaurant.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                Role = UserRoles.Customer,
                CreatedAt = DateTime.UtcNow
            }
        };

            context.Users.AddRange(users);
        }

        if (!await context.TimeSlots.AnyAsync())
        {
            var timeSlots = new List<TimeSlot>
            {
                new TimeSlot { StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(14, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(20, 0), EndTime = new TimeOnly(22, 0), IsActive = true }
            };

            context.TimeSlots.AddRange(timeSlots);
        }

        if(!await context.RestaurantConfigurations.AnyAsync())
        {
            context.RestaurantConfigurations.Add(new RestaurantConfiguration
            {
                MaxPartySize=20,
                CancellationWindowHours=2,
                AdvanceBookingDays=30
            });
        }

        if(!await context.OperatingHours.AnyAsync())
        {
            var operatingHours=new List<OperatingHours>
            {
                new OperatingHours { DayOfWeek=DayOfWeek.Sunday, IsClosed=true },
                new OperatingHours { DayOfWeek=DayOfWeek.Monday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Tuesday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Wednesday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Thursday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Friday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(23,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Saturday, OpeningTime=new TimeOnly(9,0), ClosingTime=new TimeOnly(23,0), IsClosed=false }
            };
            context.OperatingHours.AddRange(operatingHours);
        }

        await context.SaveChangesAsync();
    }
}
