using RestaurantTableReservationAPI.Repositories.Interfaces;

namespace RestaurantTableReservationAPI.Services.Validation;

public class ReservationValidationEngine : IReservationValidationEngine
{
    private readonly IRestaurantConfigRepository _configRepository;

    public ReservationValidationEngine(IRestaurantConfigRepository configRepository)
    {
        _configRepository=configRepository;
    }

    public async Task<ReservationValidationResult> ValidateReservationRequestAsync(DateOnly date, TimeOnly time, int partySize)
    {
        var config=await _configRepository.GetConfigurationAsync();
        if(config==null)
        {
            return ReservationValidationResult.Failure("System configuration is missing. Cannot process reservations.");
        }

        var requestedDateTime=date.ToDateTime(time);
        var now=DateTime.Now;

        // Rule 1: Past Date Check
        if(requestedDateTime<now)
        {
            return ReservationValidationResult.Failure("Reservations cannot be made in the past.");
        }

        // Rule 2: Minimum Notice (1 hour)
        if(requestedDateTime<now.AddHours(1))
        {
            return ReservationValidationResult.Failure("Reservations must be made at least 1 hour in advance.");
        }

        // Rule 3: Advance Booking Check
        if(requestedDateTime>now.AddDays(config.AdvanceBookingDays))
        {
            return ReservationValidationResult.Failure($"Reservations can only be made up to {config.AdvanceBookingDays} days in advance.");
        }

        // Rule 4: Party Size Check
        if(partySize>config.MaxPartySize)
        {
            return ReservationValidationResult.Failure($"Requested party size ({partySize}) exceeds the restaurant's maximum allowed party size of {config.MaxPartySize}.");
        }

        if(partySize<=0)
        {
            return ReservationValidationResult.Failure("Party size must be greater than zero.");
        }

        return ReservationValidationResult.Success();
    }
}
