namespace RestaurantTableReservationAPI.Services.Validation;

public interface IReservationValidationEngine
{
    Task<ReservationValidationResult> ValidateReservationRequestAsync(DateOnly date, TimeOnly time, int partySize);
    Task<ReservationValidationResult> ValidateDateAndPartySizeAsync(DateOnly date, int partySize);
}
