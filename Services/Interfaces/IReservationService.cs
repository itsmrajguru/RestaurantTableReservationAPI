using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(int userId, CreateReservationDto dto);
    Task<List<ReservationResponseDto>> GetCustomerUpcomingReservationsAsync(int userId);
    Task<List<ReservationResponseDto>> GetCustomerReservationHistoryAsync(int userId);
    Task<ReservationResponseDto> GetCustomerReservationByIdAsync(int reservationId, int userId);
    Task<List<ReservationResponseDto>> GetAllReservationsAsync();
    Task<ReservationResponseDto> UpdateStatusAsync(int reservationId, Models.Enums.ReservationStatus newStatus);
    Task<bool> CancelReservationAsync(int reservationId, int userId);
}
