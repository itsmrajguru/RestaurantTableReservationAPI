using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantTableReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestAuthController : ControllerBase
{
    // Anyone can access this
    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok(new{message="This is a public endpoint."});
    }

    // Must have a valid JWT token, but any role is fine
    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult GetAuthenticated()
    {
        return Ok(new{message="You are authenticated!"});
    }

    // Must have a valid JWT token AND the Customer role
    [HttpGet("customer")]
    [Authorize(Roles="Customer")]
    public IActionResult GetCustomer()
    {
        return Ok(new{message="You have access to the Customer endpoint!"});
    }

    // Must have a valid JWT token AND be either Staff or Admin
    [HttpGet("staff")]
    [Authorize(Roles="Staff,Admin")]
    public IActionResult GetStaff()
    {
        return Ok(new{message="You have access to the Staff endpoint!"});
    }

    // Must have a valid JWT token AND the Admin role
    [HttpGet("admin")]
    [Authorize(Roles="Admin")]
    public IActionResult GetAdmin()
    {
        return Ok(new{message="You have access to the Admin endpoint!"});
    }
}
