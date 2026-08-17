using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface ITimeSlotService
{
    Task<List<TimeSlotResponseDto>> GetAllTimeSlotsAsync(bool isAdmin=false);
    Task<TimeSlotResponseDto?> GetTimeSlotByIdAsync(int id, bool isAdmin=false);
    Task<TimeSlotResponseDto?> CreateTimeSlotAsync(CreateTimeSlotDto createDto);
    Task<TimeSlotResponseDto?> UpdateTimeSlotAsync(int id, UpdateTimeSlotDto updateDto);
    Task<bool> DeleteTimeSlotAsync(int id);
}
