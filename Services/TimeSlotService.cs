using AutoMapper;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public TimeSlotService(ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _timeSlotRepository=timeSlotRepository;
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
            return null; // Invalid time range
        }

        var slot=_mapper.Map<TimeSlot>(createDto);
        slot.IsActive=true;
        var createdSlot=await _timeSlotRepository.AddAsync(slot);
        return _mapper.Map<TimeSlotResponseDto>(createdSlot);
    }

    public async Task<TimeSlotResponseDto?> UpdateTimeSlotAsync(int id, UpdateTimeSlotDto updateDto)
    {
        if(updateDto.StartTime>=updateDto.EndTime)
        {
            return null; // Invalid time range
        }

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

        slot.IsActive=false;
        await _timeSlotRepository.UpdateAsync(slot);
        return true;
    }
}
