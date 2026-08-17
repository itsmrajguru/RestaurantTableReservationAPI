using AutoMapper;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.DTOs;

/* this file is only made to execute the automapper
so that we dont need to manually copy the model fields into dto fields*/
namespace RestaurantTableReservationAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Table Mappings
        CreateMap<RestaurantTable, TableResponseDto>();
        CreateMap<CreateTableDto, RestaurantTable>();
        CreateMap<UpdateTableDto, RestaurantTable>();

        // TimeSlot Mappings
        CreateMap<TimeSlot, TimeSlotResponseDto>();
        CreateMap<CreateTimeSlotDto, TimeSlot>();
        CreateMap<UpdateTimeSlotDto, TimeSlot>();

        // Reservation Mappings
        // We use ForMember to flatten nested objects (e.g. mapping r.User.Name to dto.UserName)
        CreateMap<Reservation, ReservationResponseDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table.TableNumber))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TimeSlot.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.TimeSlot.EndTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateReservationDto, Reservation>()
            .ForMember(dest => dest.Status, opt => opt.Ignore()) // Don't allow user to set status
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // RestaurantConfiguration Mappings
        CreateMap<RestaurantConfiguration, RestaurantConfigResponseDto>();
        CreateMap<UpdateRestaurantConfigDto, RestaurantConfiguration>();

        // OperatingHours Mappings
        CreateMap<OperatingHours, OperatingHoursResponseDto>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek.ToString()))
            .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => src.OpeningTime.HasValue ? src.OpeningTime.Value.ToString("HH:mm") : null))
            .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => src.ClosingTime.HasValue ? src.ClosingTime.Value.ToString("HH:mm") : null));
            
        CreateMap<UpdateOperatingHoursDto, OperatingHours>()
            .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.OpeningTime) ? (TimeOnly?)null : TimeOnly.Parse(src.OpeningTime)))
            .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ClosingTime) ? (TimeOnly?)null : TimeOnly.Parse(src.ClosingTime)))
            .ForMember(dest => dest.DayOfWeek, opt => opt.Ignore()) // Cannot update the day
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Cannot update the id
    }
}
