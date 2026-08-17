using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RestaurantConfigController : ControllerBase
{
    private readonly IRestaurantConfigService _configService;

    public RestaurantConfigController(IRestaurantConfigService configService)
    {
        _configService=configService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetConfiguration()
    {
        var config=await _configService.GetConfigurationAsync();
        if(config==null) return NotFound(new{message="Configuration not found."});
        return Ok(config);
    }

    [HttpPut]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> UpdateConfiguration([FromBody] UpdateRestaurantConfigDto updateDto)
    {
        var updatedConfig=await _configService.UpdateConfigurationAsync(updateDto);
        if(updatedConfig==null) return NotFound(new{message="Configuration not found."});
        return Ok(updatedConfig);
    }
}
