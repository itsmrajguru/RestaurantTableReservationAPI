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

    /* This is a publically accessible async method which further calls 
    configRepository and returns a dto or null*/
    public async Task<RestaurantConfigResponseDto?> GetConfigurationAsync()
    {
        var config=await _configRepository.GetConfigurationAsync();
        if(config==null) return null;
        /* This means that take config from database and map it into 
        RestaurantConfigResponseDto and return this dto to controller to the user */
        return _mapper.Map<RestaurantConfigResponseDto>(config);
    }

    
    public async Task<RestaurantConfigResponseDto?> UpdateConfigurationAsync(UpdateRestaurantConfigDto updateDto)
    {
        var config=await _configRepository.GetConfigurationAsync();
        if(config==null) return null; // Seed ensures it exists, but just in case

        /* these are 2 diffrent conditions
        here  _mapper.Map(updateDto, config); --> means
        take data from updateDto and replce into config, so config is
        changed automatically in the database
        
        and _mapper.Map<RestaurantConfigResponseDto>(config)-->means
        take new config and put it into RestaurantConfigResponseDto
        and return it to the controller to user
        */
        _mapper.Map(updateDto, config);
        await _configRepository.UpdateConfigurationAsync(config);

        return _mapper.Map<RestaurantConfigResponseDto>(config);
    }
}
