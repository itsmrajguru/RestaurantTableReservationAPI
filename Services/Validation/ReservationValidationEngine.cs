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

        // Rule 1: check whether the requested time is after the current time ?
        if(requestedDateTime<now)
        {
            return ReservationValidationResult.Failure("Reservations cannot be made in the past.");
        }

        // Rule 2: Reservations must be made before, what time ?
        if(requestedDateTime<now.AddHours(1))
        {
            return ReservationValidationResult.Failure("Reservations must be made at least 1 hour in advance.");
        }

        // Rule 3: Reservations can only be done , how many days before?
        if(requestedDateTime>now.AddDays(config.AdvanceBookingDays))
        {
            return ReservationValidationResult.Failure($"Reservations can only be made up to {config.AdvanceBookingDays} days in advance.");
        }

        // Rule 4: How many maxPartysize can be allowed ?
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

    public async Task<ReservationValidationResult> ValidateDateAndPartySizeAsync(DateOnly date, int partySize)
    {
        var config=await _configRepository.GetConfigurationAsync();
        if(config==null)
        {
            return ReservationValidationResult.Failure("System configuration is missing. Cannot process reservations.");
        }

        var today=DateOnly.FromDateTime(DateTime.Now);

        // Rule 1: check whether the requested date is before current date?
        if(date<today)
        {
            return ReservationValidationResult.Failure("Reservations cannot be made in the past.");
        }

        // Rule 3: In how many advance days,the reservations can be done?
        if(date>today.AddDays(config.AdvanceBookingDays))
        {
            return ReservationValidationResult.Failure($"Reservations can only be made up to {config.AdvanceBookingDays} days in advance.");
        }

        // Rule 4: How many maxPartysize can be allowed ?
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
