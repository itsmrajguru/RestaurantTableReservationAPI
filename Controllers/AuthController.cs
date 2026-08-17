using Microsoft.AspNetCore.Mvc;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Controllers;


[Route("api/[controller]")]
/*this api controller automatically returns 400 responses
and also helps for validation*/
[ApiController]
public class AuthController : ControllerBase
{
    // first DEPENDENCY INJECTION Class
    private readonly IAuthService _authService;
    /*Here the authController is simply asking the .NET Core
    that which class executes the IAuthService
    then the .NET DI returns the authService 
    and thus authService is used as _authService*/
    public AuthController(IAuthService authService)
    {
        _authService=authService;
    }



    [HttpPost("login")]
    /*Here Task->represents a thread to be completed
    and IActionResult -> represents the result to be returned
    when the code is executed then it returns the result
    if the login is successful then it returns the result
    if the login fails then it returns the error message
    */

    /*
    the [FromBody] attribute is used to bind the login credentials from the req.body to the LoginRequestDto object
    and LoginRequestDto is used as loginDto for easy Access.
    */
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
    {
        var result=await _authService.LoginAsync(loginDto);
        if(result==null)
        {
            return Unauthorized(new{message="Invalid email or password."});
        }
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerDto registerDto)
    {
        var isSuccess=await _authService.RegisterCustomerAsync(registerDto);
        if(!isSuccess)
        {
            return BadRequest(new{message="Email is already registered."});
        }
        return Ok(new{message="Customer registered successfully."});
    }
}
