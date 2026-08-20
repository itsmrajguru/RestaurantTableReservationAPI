using Microsoft.EntityFrameworkCore;
using RestaurantTableReservationAPI.Data;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Models.Enums;
using RestaurantTableReservationAPI.Repositories.Interfaces;

namespace RestaurantTableReservationAPI.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Reservation>> GetByUserIdAsync(int userId)
    {
        return await _context.Reservations
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReservationDate)
            .ThenBy(r => r.TimeSlot.StartTime)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetByDateAsync(DateOnly date)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .Where(r => r.ReservationDate == date)
            .OrderBy(r => r.TimeSlot.StartTime)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetAllAsync()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Table)
            .Include(r => r.TimeSlot)
            .OrderByDescending(r => r.ReservationDate)
            .ThenBy(r => r.TimeSlot.StartTime)
            .ToListAsync();
    }

    public async Task<Reservation> CreateReservationWithConcurrencyCheckAsync(Reservation reservation)
    {
        /* Start a DB transaction with Serializable isolation: the strictest level.
        This ensures that if two users try to book the same table/slot at the same time,
        their transactions are fully isolated (one waits/conflicts instead of both succeeding).
        Prevents double-booking race conditions. 'using' auto-disposes the transaction when done. */
        using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        try
        {
            // 1. Double check if booked inside the transaction lock
            var isBooked = await _context.Reservations.AnyAsync(r => 
                r.TableId == reservation.TableId && 
                r.ReservationDate == reservation.ReservationDate && 
                r.TimeSlotId == reservation.TimeSlotId &&
                (r.Status == Models.Enums.ReservationStatus.Pending || r.Status == Models.Enums.ReservationStatus.Confirmed || r.Status == Models.Enums.ReservationStatus.CheckedIn));

            if (isBooked)
            {
                throw new InvalidOperationException("DOUBLE_BOOKING_CONFLICT");
            }

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            /* Now the transaction is confirmed, changes are permanent in the database */
            await transaction.CommitAsync();
            return reservation;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task AddAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        reservation.UpdatedAt = DateTime.UtcNow;
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsTableBookedAsync(int tableId, DateOnly date, int timeSlotId)
    {
        // A table is booked if there's a Confirmed or Pending reservation for that table, date, and time slot.
        // Cancelled or NoShow means the table is free.
        return await _context.Reservations.AnyAsync(r =>
            r.TableId == tableId &&
            r.ReservationDate == date &&
            r.TimeSlotId == timeSlotId &&
            (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn));
    }
}


/* 

Line by line
using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

Teen naye concepts hain is ek line mein — todte hain:

1. Transaction kya hai:
Ek transaction database operations ka ek group hota hai jo ya toh poora successful hota hai, ya poora fail ho jaata hai — beech mein kuch "half-done" nahi rehta. Jaise: "table check karo, aur reservation save karo" — yeh dono operations ek single unit ki tarah treat honge. Agar beech mein kuch galat hua, dono undo ho jaayenge, jaise kuch hua hi nahi.

2. BeginTransactionAsync(...) kya karta hai:
Yeh database ko bolta hai: "ab se jo bhi operations main karunga, unhe ek transaction ke andar treat karo — jab tak main explicitly na bolun 'confirm karo' (commit), yeh changes permanent nahi honge."

3. IsolationLevel.Serializable — yeh sabse important part hai:
Jab multiple transactions same waqt chal rahe hote hain (jaise User A aur User B dono ek saath try kar rahe hain), toh database ko decide karna padta hai — "kya ek transaction doosre transaction ka beech ka, abhi-tak-uncommitted data dekh sakta hai?"

Serializable sabse strict/safe level hai — iska matlab: "agar do transactions ek hi data pe kaam kar rahe hain, toh unhe ek dusre ke saath conflict na karne do — jaise ki woh ek-ek karke (serially), ek ke baad ek, chal rahe hon, chaahe woh technically same waqt shuru hue hon."

Simple bhasha mein: yeh database ko bolta hai — "agar User A aur User B dono same table ko same waqt check-and-book karne ki koshish kar rahe hain, toh unhe ek dusre se completely block/isolate kar do — jab tak User A ka poora transaction khatam na ho jaaye, User B ka transaction wait karega (ya conflict detect karega)."

using var transaction = ... — using keyword ka matlab hai: jab yeh method khatam ho (chaahe successfully ho ya error se), transaction object automatically cleanup/dispose ho jaayega — resource leak nahi hoga.
*/

/* ACTUAL WORKING MECHANISM OF DOUBLE_BOOKING_CONFLICT */

/* 
Kaise decide hota hai — actual mechanism

Jab BeginTransactionAsync call hota hai, dono users ka request database tak network ke through jaata hai. Chaahe dono users ne apne screen pe "Book" button ekdum same second pe click kiya ho, physically unke requests database server tak kabhi bhi ek dum exact same nanosecond pe nahi pahunchte — hamesha koi na koi microscopic time difference hota hai (network speed, server processing, jitna bhi chhota ho) jisse ek request doosre se thoda sa pehle pahunchta hai.

Jo request pehle pahunchta hai, database us table/row pe pehle lock le leta hai. Doosra request jab aata hai aur wahi data touch karna chahta hai, database use wait karwa deta hai (ya, isolation level ke hisaab se, seedha conflict/error de deta hai) — jab tak pehla transaction apna kaam khatam (commit ya rollback) na kar le.

Serializable isolation level mein exactly kya hota hai

Do tareeke se yeh handle ho sakta hai (database engine pe depend karta hai, jaise PostgreSQL vs SQL Server thoda alag tareeke se implement karte hain):

Approach 1 — Blocking/Waiting:

User A ka transaction pehle pahuncha → usne lock le liya us table+date+slot combination pe.
User B ka transaction thoda der baad pahuncha → jab woh same data check/insert karne ki koshish karta hai, database use wait karwa deta hai (jaise ek line mein khada karwa diya).
Jaise hi User A CommitAsync() (ya RollbackAsync()) karta hai, lock release hota hai, aur ab User B ka transaction aage badh sakta hai — is waqt tak User A ka data already saved hai, toh User B ka isBooked check true aayega.

Approach 2 — Immediate Conflict Detection:

Kuch databases, Serializable level pe, dusre transaction ko wait nahi karwate — balki turant ek "serialization failure" error de dete hain jaise hi conflict detect ho, taaki application (humara catch block) usse turant handle kar sake.

Dono approaches ka end result same hota hai humare code ke liye — jo transaction pehle commit hota hai, uski booking "win" karti hai, aur doosra transaction ko humara DOUBLE_BOOKING_CONFLICT exception milta hai.
*/