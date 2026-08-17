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
        _userRepository=userRepository;
        _config=config;
    }



    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginDto)
    {
        // 1. Find user by email
        var user=await _userRepository.GetByEmailAsync(loginDto.Email);
        if(user==null)
        {
            return null; // User not found
        }

        // 2. Verify password
        bool isPasswordValid=BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if(!isPasswordValid)
        {
            return null; // Invalid password
        }

        // 3. Generate JWT Token
        var token=GenerateJwtToken(user);

        // 4. Generate Refresh Token
        var refreshToken=GenerateRefreshToken();
        user.RefreshToken=refreshToken;
        user.RefreshTokenExpiry=DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpiryDays"]!));

        // 5. Save refresh token to database
        await _userRepository.UpdateAsync(user);

        // 6. Return response
        return new AuthResponseDto
        {
            Token=token,
            RefreshToken=refreshToken,
            Role=user.Role,
            Name=user.Name
        };
    }

    public async Task<bool> RegisterCustomerAsync(RegisterCustomerDto registerDto)
    {
        // 1. Check if email already exists
        var existingUser=await _userRepository.GetByEmailAsync(registerDto.Email);
        if(existingUser!=null)
        {
            return false; // Email is already taken
        }

        // 2. Hash the password
        string hashedPassword=BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // 3. Create the new User object (Role is forced to Customer)
        var newUser=new User
        {
            Name=registerDto.Name,
            Email=registerDto.Email,
            PasswordHash=hashedPassword,
            Role=RestaurantTableReservationAPI.Models.UserRoles.Customer,
            CreatedAt=DateTime.UtcNow
        };

        // 4. Save to database
        await _userRepository.AddAsync(newUser);
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials=new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims=new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token=new JwtSecurityToken(
            issuer:_config["Jwt:Issuer"],
            audience:_config["Jwt:Audience"],
            claims:claims,
            expires:DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!)),
            signingCredentials:credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber=new byte[32];
        using var rng=RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
