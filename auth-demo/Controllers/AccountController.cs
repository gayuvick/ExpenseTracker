using auth_demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth_demo.Services;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;

    public AccountController(UserService userService, IConfiguration configuration, TokenService tokenService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _tokenService = tokenService;
        _logger = logger;
    }

      [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel model)
    {
        var user = await _userService.FindByUsernameAsync(model.Username);
        if (user == null || user.Password != model.Password)
        {
            _logger.LogWarning("Invalid login attempt for username: {Username}", model.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Generate the token using the TokenService
        var tokenString = _tokenService.GenerateToken(model.Username);

        HttpContext.Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
        {
            HttpOnly = true, // Prevents JavaScript access to the cookie
            Secure = true, // Ensures the cookie is only sent over HTTPS
            SameSite = SameSiteMode.Strict, // Helps prevent CSRF
            Expires = DateTime.UtcNow.AddHours(1) // Set the expiration time to match the token's expiration
        });
        _logger.LogInformation("User {Username} logged in successfully", model.Username);
        return Ok(new { message = "Login successful" });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
    {
        var existingUser = await _userService.FindByUsernameAsync(model.Username);
        if (existingUser != null)
        {
            return Conflict(new { message = "User already exists" });
        }

        var newUser = new User { Username = model.Username, Password = model.Password };
        _userService.AddUser(newUser);
        return CreatedAtAction(nameof(Login), new { username = model.Username });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Placeholder for client-side token removal logic
        HttpContext.Response.Cookies.Delete("AuthToken");
        _logger.LogInformation("User logged out successfully");
        return Ok(new { message = "Logout successful" });
    }
}

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

