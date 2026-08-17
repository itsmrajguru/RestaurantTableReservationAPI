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
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table.TableNumber))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TimeSlot.StartTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateReservationDto, Reservation>()
            .ForMember(dest => dest.Status, opt => opt.Ignore()) // Don't allow user to set status
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
