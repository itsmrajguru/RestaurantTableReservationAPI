using AutoMapper;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly IMapper _mapper;

    public TimeSlotService(ITimeSlotRepository timeSlotRepository, IOperatingHoursRepository operatingHoursRepository, IMapper mapper)
    {
        _timeSlotRepository=timeSlotRepository;
        _operatingHoursRepository=operatingHoursRepository;
        _mapper=mapper;
    }

    public async Task<List<TimeSlotResponseDto>> GetAllTimeSlotsAsync(bool isAdmin=false)
    {
        var slots=await _timeSlotRepository.GetAllAsync(isAdmin);
        return _mapper.Map<List<TimeSlotResponseDto>>(slots);
    }

    public async Task<TimeSlotResponseDto?> GetTimeSlotByIdAsync(int id, bool isAdmin=false)
    {
        var slot=await _timeSlotRepository.GetByIdAsync(id, isAdmin);
        if(slot==null) return null;
        return _mapper.Map<TimeSlotResponseDto>(slot);
    }

    public async Task<TimeSlotResponseDto?> CreateTimeSlotAsync(CreateTimeSlotDto createDto)
    {
        if(createDto.StartTime>=createDto.EndTime)
        {
            throw new ArgumentException("StartTime must be before EndTime.");
        }

        await EnsureSlotFitsOperatingHoursAsync(createDto.StartTime, createDto.EndTime);

        var slot=_mapper.Map<TimeSlot>(createDto);
        slot.IsActive=true;
        var createdSlot=await _timeSlotRepository.AddAsync(slot);
        return _mapper.Map<TimeSlotResponseDto>(createdSlot);
    }

    public async Task<TimeSlotResponseDto?> UpdateTimeSlotAsync(int id, UpdateTimeSlotDto updateDto)
    {
        if(updateDto.StartTime>=updateDto.EndTime)
        {
            throw new ArgumentException("StartTime must be before EndTime.");
        }

        await EnsureSlotFitsOperatingHoursAsync(updateDto.StartTime, updateDto.EndTime);

        var slot=await _timeSlotRepository.GetByIdAsync(id, true);
        if(slot==null) return null;

        _mapper.Map(updateDto, slot);
        await _timeSlotRepository.UpdateAsync(slot);

        return _mapper.Map<TimeSlotResponseDto>(slot);
    }

    public async Task<bool> DeleteTimeSlotAsync(int id)
    {
        var slot=await _timeSlotRepository.GetByIdAsync(id, true);
        if(slot==null) return false;

        slot.IsActive=false; //soft delete
        await _timeSlotRepository.UpdateAsync(slot);
        return true;
    }

    private async Task EnsureSlotFitsOperatingHoursAsync(TimeOnly startTime, TimeOnly endTime)
    {
        var hours=await _operatingHoursRepository.GetAllAsync();
        var openDays=hours.Where(h=>!h.IsClosed && h.OpeningTime.HasValue && h.ClosingTime.HasValue).ToList();

        if(!openDays.Any())
        {
            throw new ArgumentException("The restaurant has no open days configured. Cannot create time slots.");
        }

        
        // The slot must fit within the opening and closing times of at least ONE open day
        bool fitsAnyDay=openDays.Any(day => startTime>=day.OpeningTime!.Value && endTime<=day.ClosingTime!.Value);

        if(!fitsAnyDay)
        {
            throw new ArgumentException($"The proposed time slot ({startTime} - {endTime}) does not fit within the operating hours of any day.");
        }
    }
}
