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

    /* controller->service->repository->dbcontext
    see here dbcontext will return the RestaurantConfigurations
    will be returned to repository,then repository will return
    to the service and service to the controller , and controller to the user */
    [HttpGet]
    [AllowAnonymous] //AllowAnonymous-->means anyone can access it 

    /*note :Task<T> ka matlab hai ki jab kaam complete hoga, tab type T ka result milega.
    ex...here,
    GetConfiguration()= async function
    IactionResult= result, that will be sent to the user back*/
    public async Task<IActionResult> GetConfiguration()
    {
        var config=await _configService.GetConfigurationAsync();
        if(config==null) return NotFound(new{message="Configuration not found."});
        return Ok(config);
    }

    /* When someone sends a PUT request, take the JSON from its body, convert it into UpdateRestaurantConfigDto, and store it in updateDto. */
    [HttpPut]
    [Authorize(Roles="Admin")] // Only Admin can access it
    public async Task<IActionResult> UpdateConfiguration([FromBody] UpdateRestaurantConfigDto updateDto)
    {
        var updatedConfig=await _configService.UpdateConfigurationAsync(updateDto);
        if(updatedConfig==null) return NotFound(new{message="Configuration not found."});
        return Ok(updatedConfig);
    }
}
