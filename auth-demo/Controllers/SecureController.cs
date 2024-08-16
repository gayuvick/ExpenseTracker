using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // This attribute ensures that only authenticated users can access these endpoints
public class SecureController : ControllerBase
{
    [HttpGet("secure-data")]
    public IActionResult GetSecureData()
    {
        return Ok(new { message = "This is protected data" });
    }

    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var user = User.Identity?.Name; // Get the username from the token claims
        return Ok(new { message = $"Hello, {user}" });
    }
}


