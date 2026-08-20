using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;
using RestaurantTableReservationAPI.Services.Validation;

namespace RestaurantTableReservationAPI.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IReservationValidationEngine _validationEngine;
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IReservationRepository _reservationRepository;

    public AvailabilityService(
        IReservationValidationEngine validationEngine,
        IOperatingHoursRepository operatingHoursRepository,
        ITimeSlotRepository timeSlotRepository,
        ITableRepository tableRepository,
        IReservationRepository reservationRepository)
    {
        _validationEngine=validationEngine;
        _operatingHoursRepository=operatingHoursRepository;
        _timeSlotRepository=timeSlotRepository;
        _tableRepository=tableRepository;
        _reservationRepository=reservationRepository;
    }

    public async Task<AvailabilityResponseDto> GetAvailabilityAsync(DateOnly date, int partySize)
    {
        var response=new AvailabilityResponseDto{ Date=date, PartySize=partySize };

        // 1. Validate Date and Party Size
        var validationResult=await _validationEngine.ValidateDateAndPartySizeAsync(date, partySize);
        if(!validationResult.IsValid)
        {
            // For an API endpoint, we might want to throw an exception so the controller returns 400.
            // But since this is a search, returning empty availability is also an option.
            // Let's throw ArgumentException so the user knows WHY it's not available (e.g. party size too large).
            throw new ArgumentException(validationResult.ErrorMessage);
        }

        // 2. Check Operating Hours for the specific day
        /* 2a. Get all operating hours records, then find the one matching
the requested date's day of week (e.g. if date is a Monday, find Monday's record)*/ 
        var operatingHoursList=await _operatingHoursRepository.GetAllAsync();
        var dayHours=operatingHoursList.FirstOrDefault(h=>h.DayOfWeek==date.DayOfWeek);

        if(dayHours==null || dayHours.IsClosed || !dayHours.OpeningTime.HasValue || !dayHours.ClosingTime.HasValue)
        {
            return response; //it will return [](empty box-->that means no slot available)
        }

        // 3. get all Time Slots of the requested date
        var allTimeSlots=await _timeSlotRepository.GetAllAsync();
        var validTimeSlots=allTimeSlots.Where(ts => 
            ts.StartTime >= dayHours.OpeningTime.Value && 
            ts.EndTime <= dayHours.ClosingTime.Value
        ).ToList();

        /* 3b. If date is today,
         filter out past time slots (with 1-hour minimum notice)*/
        if(date==DateOnly.FromDateTime(DateTime.Now))
        {
            var now=TimeOnly.FromDateTime(DateTime.Now);
            var minNoticeTime=now.AddHours(1);

            //this means,return the only valid slots, whose timing is after the minNoticeTime
             
            validTimeSlots=validTimeSlots.Where(ts=>ts.StartTime >= minNoticeTime).ToList();
        }

        if(!validTimeSlots.Any()) return response;

        // 4. Filter Tables by Capacity
        var allTables=await _tableRepository.GetAllAsync();
        var capableTables=allTables.Where(t=>t.Capacity>=partySize).ToList();

        if(!capableTables.Any()) return response; // No table is big enough

        // 5. Check Conflicts
        var existingReservations=await _reservationRepository.GetByDateAsync(date);
        
        // Exclude cancelled and no-show
        var activeReservations=existingReservations.Where(r=>
            r.Status==Models.Enums.ReservationStatus.Confirmed || 
            r.Status==Models.Enums.ReservationStatus.Pending).ToList();

        foreach(var slot in validTimeSlots)
        {
            // Find tables that are NOT booked for this slot
            var bookedTableIds=activeReservations
                .Where(r=>r.TimeSlotId==slot.Id)
                .Select(r=>r.TableId)
                .ToHashSet();

            bool isSlotAvailable=capableTables.Any(t=>!bookedTableIds.Contains(t.Id));

            if(isSlotAvailable)
            {
                response.AvailableTimeSlots.Add(new AvailableTimeSlotDto
                {
                    TimeSlotId=slot.Id,
                    StartTime=slot.StartTime,
                    EndTime=slot.EndTime
                });
            }
        }

        return response;
    }
}
