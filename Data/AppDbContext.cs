using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Models;

namespace RestaurantTableReservationAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RestaurantTable> Tables { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<RestaurantConfiguration> RestaurantConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<RestaurantTable>()
            .HasIndex(t => t.TableNumber)
            .IsUnique();

        // Prevent cascade delete — we want to keep reservation records
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => r.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.TimeSlot)
            .WithMany(ts => ts.Reservations)
            .HasForeignKey(r => r.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
