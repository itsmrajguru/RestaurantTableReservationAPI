using AutoMapper;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Services;

public class OperatingHoursService : IOperatingHoursService
{
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly IMapper _mapper;

    public OperatingHoursService(IOperatingHoursRepository operatingHoursRepository, IMapper mapper)
    {
        _operatingHoursRepository=operatingHoursRepository;
        _mapper=mapper;
    }

    public async Task<List<OperatingHoursResponseDto>> GetAllOperatingHoursAsync()
    {
        var hours=await _operatingHoursRepository.GetAllAsync();
        return _mapper.Map<List<OperatingHoursResponseDto>>(hours);
    }

    public async Task<OperatingHoursResponseDto?> GetOperatingHoursByIdAsync(int id)
    {
        var hours=await _operatingHoursRepository.GetByIdAsync(id);
        if(hours==null) return null;
        return _mapper.Map<OperatingHoursResponseDto>(hours);
    }

    public async Task<OperatingHoursResponseDto?> GetOperatingHoursByDayAsync(DayOfWeek day)
    {
        var hours = await _operatingHoursRepository.GetByDayAsync(day);
        if(hours==null) return null;
        return _mapper.Map<OperatingHoursResponseDto>(hours);
    }

    public async Task<OperatingHoursResponseDto?> UpdateOperatingHoursAsync(int id, UpdateOperatingHoursDto updateDto)
    {
        //1. check the existing data
        var hours=await _operatingHoursRepository.GetByIdAsync(id);
        if(hours==null) return null;

        //2. validating opening and closing time
        /* We are cheking whether the Because restaurant is open ,if open,
         we need to make sure opening and closing times are valid. */
        if(!updateDto.IsClosed)
        {
            if(string.IsNullOrEmpty(updateDto.OpeningTime) || string.IsNullOrEmpty(updateDto.ClosingTime))
            {
                throw new ArgumentException("Opening and Closing times are required when the restaurant is not closed.");
            }

            var openTime=TimeOnly.Parse(updateDto.OpeningTime);
            var closeTime=TimeOnly.Parse(updateDto.ClosingTime);

            if(openTime>=closeTime)
            {
                throw new ArgumentException("Opening time must be earlier than closing time.");
            }
        }
        else
        {
            // If closed, clear the times
            updateDto.OpeningTime=null;
            updateDto.ClosingTime=null;
        }

        _mapper.Map(updateDto, hours);
        await _operatingHoursRepository.UpdateAsync(hours);

        return _mapper.Map<OperatingHoursResponseDto>(hours);
    }
}
