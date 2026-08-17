using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;
using RestaurantTableReservationAPI.Services.Validation;

namespace RestaurantTableReservationAPI.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly IReservationValidationEngine _validationEngine;

    public ReservationService(
        IReservationRepository reservationRepository,
        ITableRepository tableRepository,
        ITimeSlotRepository timeSlotRepository,
        IOperatingHoursRepository operatingHoursRepository,
        IReservationValidationEngine validationEngine)
    {
        _reservationRepository=reservationRepository;
        _tableRepository=tableRepository;
        _timeSlotRepository=timeSlotRepository;
        _operatingHoursRepository=operatingHoursRepository;
        _validationEngine=validationEngine;
    }

    public async Task<ReservationResponseDto> CreateReservationAsync(int userId, CreateReservationDto dto)
    {
        // 1. Validate Date and Party Size globally
        var validationResult=await _validationEngine.ValidateDateAndPartySizeAsync(dto.ReservationDate, dto.PartySize);
        if(!validationResult.IsValid)
        {
            throw new ArgumentException(validationResult.ErrorMessage);
        }

        // 2. Validate TimeSlot
        var timeSlot=await _timeSlotRepository.GetByIdAsync(dto.TimeSlotId);
        if(timeSlot==null || !timeSlot.IsActive)
        {
            throw new ArgumentException("The selected time slot is invalid or no longer active.");
        }

        // 3. Validate Operating Hours
        var operatingHoursList=await _operatingHoursRepository.GetAllAsync();
        var dayHours=operatingHoursList.FirstOrDefault(h=>h.DayOfWeek==dto.ReservationDate.DayOfWeek);
        if(dayHours==null || dayHours.IsClosed || !dayHours.OpeningTime.HasValue || !dayHours.ClosingTime.HasValue)
        {
            throw new ArgumentException("The restaurant is closed on the selected date.");
        }

        if(timeSlot.StartTime<dayHours.OpeningTime.Value || timeSlot.EndTime>dayHours.ClosingTime.Value)
        {
            throw new ArgumentException("The selected time slot falls outside of the restaurant's operating hours for this date.");
        }

        // Minimum notice for today
        if(dto.ReservationDate==DateOnly.FromDateTime(DateTime.Now))
        {
            if(timeSlot.StartTime<TimeOnly.FromDateTime(DateTime.Now).AddHours(1))
            {
                throw new ArgumentException("Reservations must be made at least 1 hour in advance.");
            }
        }

        // 4. Auto-Assign Table
        var allTables=await _tableRepository.GetAllAsync();
        
        // Find tables that are big enough, sort them by capacity ascending (to save larger tables for larger parties)
        var capableTables=allTables
            .Where(t=>t.Capacity>=dto.PartySize)
            .OrderBy(t=>t.Capacity)
            .ToList();

        if(!capableTables.Any())
        {
            throw new ArgumentException($"No tables available that can accommodate a party of {dto.PartySize}.");
        }

        RestaurantTable? assignedTable=null;
        
        foreach(var table in capableTables)
        {
            bool isBooked=await _reservationRepository.IsTableBookedAsync(table.Id, dto.ReservationDate, dto.TimeSlotId);
            if(!isBooked)
            {
                assignedTable=table;
                break;
            }
        }

        if(assignedTable==null)
        {
            throw new ArgumentException("No tables are available for the selected date and time slot.");
        }

        // 5. Create the Reservation
        var reservation=new Reservation
        {
            UserId=userId,
            TableId=assignedTable.Id,
            TimeSlotId=timeSlot.Id,
            ReservationDate=dto.ReservationDate,
            PartySize=dto.PartySize,
            Notes=dto.Notes,
            Status=Models.Enums.ReservationStatus.Pending,
            CreatedAt=DateTime.UtcNow
        };

        await _reservationRepository.AddAsync(reservation);

        // Fetch it again to get navigation properties for the DTO
        var createdReservation=await _reservationRepository.GetByIdAsync(reservation.Id);

        return MapToDto(createdReservation!);
    }

    public async Task<List<ReservationResponseDto>> GetCustomerReservationsAsync(int userId)
    {
        var reservations=await _reservationRepository.GetByUserIdAsync(userId);
        return reservations.Select(MapToDto).ToList();
    }

    public async Task<List<ReservationResponseDto>> GetAllReservationsAsync()
    {
        var reservations=await _reservationRepository.GetAllAsync();
        return reservations.Select(MapToDto).ToList();
    }

    private ReservationResponseDto MapToDto(Reservation r)
    {
        return new ReservationResponseDto
        {
            Id=r.Id,
            ReservationDate=r.ReservationDate,
            Status=r.Status.ToString(),
            PartySize=r.PartySize,
            Notes=r.Notes,
            TableNumber=r.Table?.TableNumber ?? "Unknown",
            StartTime=r.TimeSlot?.StartTime ?? TimeOnly.MinValue,
            EndTime=r.TimeSlot?.EndTime ?? TimeOnly.MinValue,
            CustomerName=r.User?.Name ?? "Unknown",
            CustomerEmail=r.User?.Email ?? "Unknown"
        };
    }
}
