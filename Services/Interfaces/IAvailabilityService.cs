using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface IAvailabilityService
{
    Task<AvailabilityResponseDto> GetAvailabilityAsync(DateOnly date, int partySize);
}
