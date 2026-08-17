using AutoMapper;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Services;

public class RestaurantConfigService : IRestaurantConfigService
{
    private readonly IRestaurantConfigRepository _configRepository;
    private readonly IMapper _mapper;

    public RestaurantConfigService(IRestaurantConfigRepository configRepository, IMapper mapper)
    {
        _configRepository=configRepository;
        _mapper=mapper;
    }

    public async Task<RestaurantConfigResponseDto?> GetConfigurationAsync()
    {
        var config=await _configRepository.GetConfigurationAsync();
        if(config==null) return null;
        return _mapper.Map<RestaurantConfigResponseDto>(config);
    }

    public async Task<RestaurantConfigResponseDto?> UpdateConfigurationAsync(UpdateRestaurantConfigDto updateDto)
    {
        var config=await _configRepository.GetConfigurationAsync();
        if(config==null) return null; // Seed ensures it exists, but just in case

        _mapper.Map(updateDto, config);
        await _configRepository.UpdateConfigurationAsync(config);

        return _mapper.Map<RestaurantConfigResponseDto>(config);
    }
}
