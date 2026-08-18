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
                new TimeSlot { StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(14, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(16, 0), EndTime = new TimeOnly(18, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(20, 0), EndTime = new TimeOnly(22, 0), IsActive = true },
                new TimeSlot { StartTime = new TimeOnly(22, 0), EndTime = new TimeOnly(23, 0), IsActive = true } // Late slot for Saturday
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
                // Sunday: 10:00 AM to 8:00 PM
                new OperatingHours { DayOfWeek=DayOfWeek.Sunday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(20,0), IsClosed=false },
                // Monday to Friday: 10:00 AM to 10:00 PM
                new OperatingHours { DayOfWeek=DayOfWeek.Monday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Tuesday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Wednesday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Thursday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                new OperatingHours { DayOfWeek=DayOfWeek.Friday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(22,0), IsClosed=false },
                // Saturday: 10:00 AM to 11:00 PM
                new OperatingHours { DayOfWeek=DayOfWeek.Saturday, OpeningTime=new TimeOnly(10,0), ClosingTime=new TimeOnly(23,0), IsClosed=false }
            };
            context.OperatingHours.AddRange(operatingHours);
        }

        if(!await context.Tables.AnyAsync())
        {
            var tables=new List<RestaurantTable>
            {
                new RestaurantTable { TableNumber="T1", Capacity=2, Description="Cozy window seat for two" },
                new RestaurantTable { TableNumber="T2", Capacity=2, Description="Quiet corner table" },
                new RestaurantTable { TableNumber="T3", Capacity=4, Description="Central booth" },
                new RestaurantTable { TableNumber="T4", Capacity=4, Description="Patio table" },
                new RestaurantTable { TableNumber="T5", Capacity=6, Description="Large family table" }
            };
            context.Tables.AddRange(tables);
        }

        await context.SaveChangesAsync();

        // Ensure new users are seeded
        var newUsersToSeed = new List<User>
        {
            new User { Name = "Pratik", Email = "pratik@restaurant.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pratik@123"), Role = UserRoles.Customer, CreatedAt = DateTime.UtcNow },
            new User { Name = "Vikrant", Email = "vikrant@restaurant.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("vikrant@123"), Role = UserRoles.Customer, CreatedAt = DateTime.UtcNow },
            new User { Name = "Eshaan", Email = "eshaan@restaurant.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("eshaan@123"), Role = UserRoles.Customer, CreatedAt = DateTime.UtcNow },
            new User { Name = "Aryan", Email = "aryan@restaurant.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("aryan@123"), Role = UserRoles.Customer, CreatedAt = DateTime.UtcNow },
            new User { Name = "Harshal", Email = "harshal@restaurant.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("harshal@123"), Role = UserRoles.Customer, CreatedAt = DateTime.UtcNow }
        };

        bool newUsersAdded = false;
        foreach (var newUser in newUsersToSeed)
        {
            if (!await context.Users.AnyAsync(u => u.Email == newUser.Email))
            {
                context.Users.Add(newUser);
                newUsersAdded = true;
            }
        }
        
        if (newUsersAdded)
        {
            await context.SaveChangesAsync();
        }

        // Only seed reservations if Pratik has no reservations (to avoid duplicate seeding on every restart)
        var pratik = await context.Users.FirstOrDefaultAsync(u => u.Email == "pratik@restaurant.com");
        if (pratik != null && !await context.Reservations.AnyAsync(r => r.UserId == pratik.Id))
        {
            var vikrant = await context.Users.FirstAsync(u => u.Email == "vikrant@restaurant.com");
            var eshaan = await context.Users.FirstAsync(u => u.Email == "eshaan@restaurant.com");
            var aryan = await context.Users.FirstAsync(u => u.Email == "aryan@restaurant.com");
            var harshal = await context.Users.FirstAsync(u => u.Email == "harshal@restaurant.com");
            
            var allUsers = new[] { pratik, vikrant, eshaan, aryan, harshal };
            var notes = new[] { "Birthday celebration!", "Anniversary dinner", "Business meeting", "Please arrange a corner table", "Looking forward to trying the new menu", "Allergic to peanuts", "Window seat requested" };
            var rnd = new Random(42);

            var tables = await context.Tables.ToListAsync();
            var slots = await context.TimeSlots.ToListAsync();
            
            var reservations = new List<Reservation>();

            // For the next 10 days
            for (int i = 0; i <= 10; i++)
            {
                var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i));
                
                // Book 3-4 random tables per day
                for(int j = 0; j < 4; j++)
                {
                    var user = allUsers[rnd.Next(allUsers.Length)];
                    var table = tables[rnd.Next(tables.Count)];
                    var slot = slots[rnd.Next(slots.Count)];
                    
                    if (!reservations.Any(r => r.ReservationDate == date && r.TimeSlotId == slot.Id && r.TableId == table.Id) &&
                        !await context.Reservations.AnyAsync(r => r.ReservationDate == date && r.TimeSlotId == slot.Id && r.TableId == table.Id))
                    {
                        reservations.Add(new Reservation
                        {
                            UserId = user.Id,
                            TableId = table.Id,
                            TimeSlotId = slot.Id,
                            ReservationDate = date,
                            PartySize = rnd.Next(1, table.Capacity + 1),
                            Notes = notes[rnd.Next(notes.Length)],
                            Status = Models.Enums.ReservationStatus.Confirmed
                        });
                    }
                }
            }
            context.Reservations.AddRange(reservations);
            await context.SaveChangesAsync();
        }
    }
}
