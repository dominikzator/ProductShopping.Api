using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Models.Identity;
using ProductShopping.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductShopping.Infrastructure.Services;

public class JWTService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtOptions) : IJWTService
{
    public async Task<string> GenerateToken(UserDto userDTO)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userDTO.Id),
            new Claim(JwtRegisteredClaimNames.Email, userDTO.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, userDTO.FullName)
        };

        var user = await userManager.FindByEmailAsync(userDTO.Email);

        // Set user role claims
        var roles = await userManager.GetRolesAsync(user!);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims = claims.Union(roleClaims).ToList();

        // Set JWT Key credentials
        var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

        //Create an encoded token
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtOptions.Value.DurationInMinutes)),
            signingCredentials: credentials
            );

        // Return token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
