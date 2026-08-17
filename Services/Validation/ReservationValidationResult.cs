namespace RestaurantTableReservationAPI.Services.Validation;

public class ReservationValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public static ReservationValidationResult Success() => new ReservationValidationResult { IsValid=true };
    public static ReservationValidationResult Failure(string message) => new ReservationValidationResult { IsValid=false, ErrorMessage=message };
}
