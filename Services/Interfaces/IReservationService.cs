using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(int userId, CreateReservationDto dto);
    Task<List<ReservationResponseDto>> GetCustomerUpcomingReservationsAsync(int userId);
    Task<List<ReservationResponseDto>> GetCustomerReservationHistoryAsync(int userId);
    Task<ReservationResponseDto> GetCustomerReservationByIdAsync(int reservationId, int userId);
    Task<List<ReservationResponseDto>> GetAllReservationsAsync();
    Task<ReservationResponseDto> ConfirmReservationAsync(int reservationId);
    Task<ReservationResponseDto> CheckInAsync(int reservationId);
    Task<ReservationResponseDto> MarkNoShowAsync(int reservationId);
    Task<ReservationResponseDto> CompleteReservationAsync(int reservationId);
    Task<ReservationResponseDto> HandleWalkInAsync(int userId, CreateWalkInDto dto);
    Task<bool> CancelReservationAsync(int reservationId, int userId);
}
