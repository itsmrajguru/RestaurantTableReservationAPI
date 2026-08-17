using System.ComponentModel.DataAnnotations;

namespace RestaurantTableReservationAPI.DTOs;

public class TimeSlotResponseDto
{
    public int Id{get;set;}
    public TimeOnly StartTime{get;set;}
    public TimeOnly EndTime{get;set;}
    public bool IsActive{get;set;}
}

public class CreateTimeSlotDto
{
    [Required]
    public TimeOnly StartTime{get;set;}

    [Required]
    public TimeOnly EndTime{get;set;}
}

public class UpdateTimeSlotDto
{
    [Required]
    public TimeOnly StartTime{get;set;}

    [Required]
    public TimeOnly EndTime{get;set;}

    public bool IsActive{get;set;}
}
