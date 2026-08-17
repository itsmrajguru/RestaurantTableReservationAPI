using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    // First Ever DI Class
    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
<<<<<<< HEAD
        _userRepository=userRepository;
        _config=config;
=======
        _userRepository = userRepository;
        _config = config;
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
    }



    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginDto)
    {
        // 1. Find user by email
<<<<<<< HEAD
        var user=await _userRepository.GetByEmailAsync(loginDto.Email);
        if(user==null)
=======
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
        {
            return null; // User not found
        }

        // 2. Verify password
<<<<<<< HEAD
        bool isPasswordValid=BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if(!isPasswordValid)
=======
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if (!isPasswordValid)
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
        {
            return null; // Invalid password
        }

        // 3. Generate JWT Token
<<<<<<< HEAD
        var token=GenerateJwtToken(user);

        // 4. Generate Refresh Token
        var refreshToken=GenerateRefreshToken();
        user.RefreshToken=refreshToken;
        user.RefreshTokenExpiry=DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpiryDays"]!));
=======
        var token = GenerateJwtToken(user);

        // 4. Generate Refresh Token
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpiryDays"]!));
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3

        // 5. Save refresh token to database
        await _userRepository.UpdateAsync(user);

        // 6. Return response
        return new AuthResponseDto
        {
<<<<<<< HEAD
            Token=token,
            RefreshToken=refreshToken,
            Role=user.Role,
            Name=user.Name
=======
            Token = token,
            RefreshToken = refreshToken,
            Role = user.Role,
            Name = user.Name
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
        };
    }

    public async Task<bool> RegisterCustomerAsync(RegisterCustomerDto registerDto)
    {
        // 1. Check if email already exists
<<<<<<< HEAD
        var existingUser=await _userRepository.GetByEmailAsync(registerDto.Email);
        if(existingUser!=null)
=======
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
        {
            return false; // Email is already taken
        }

        // 2. Hash the password
<<<<<<< HEAD
        string hashedPassword=BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // 3. Create the new User object (Role is forced to Customer)
        var newUser=new User
        {
            Name=registerDto.Name,
            Email=registerDto.Email,
            PasswordHash=hashedPassword,
            Role=RestaurantTableReservationAPI.Models.UserRoles.Customer,
            CreatedAt=DateTime.UtcNow
=======
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // 3. Create the new User object (Role is forced to Customer)
        var newUser = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = hashedPassword,
            Role = RestaurantTableReservationAPI.Models.UserRoles.Customer,
            CreatedAt = DateTime.UtcNow
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
        };

        // 4. Save to database
        await _userRepository.AddAsync(newUser);
        return true;
    }

    private string GenerateJwtToken(User user)
    {
<<<<<<< HEAD
        var securityKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials=new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims=new[]
=======
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, user.Name)
        };

<<<<<<< HEAD
        var token=new JwtSecurityToken(
            issuer:_config["Jwt:Issuer"],
            audience:_config["Jwt:Audience"],
            claims:claims,
            expires:DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!)),
            signingCredentials:credentials);
=======
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!)),
            signingCredentials: credentials);
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
<<<<<<< HEAD
        var randomNumber=new byte[32];
        using var rng=RandomNumberGenerator.Create();
=======
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
>>>>>>> 0b0ece213a8d0b5c424456df47fd39ccb027f9a3
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
