using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface IOperatingHoursService
{
    Task<List<OperatingHoursResponseDto>> GetAllOperatingHoursAsync();
    Task<OperatingHoursResponseDto?> GetOperatingHoursByIdAsync(int id);
    Task<OperatingHoursResponseDto?> UpdateOperatingHoursAsync(int id, UpdateOperatingHoursDto updateDto);
}
