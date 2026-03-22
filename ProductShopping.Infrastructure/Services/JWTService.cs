using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductShopping.Application.Models.Identity;
using ProductShopping.Identity.Models;
using ProductShopping.Infrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductShopping.Infrastructure.Services;

public class JWTService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtOptions) : IJWTService
{
    public async Task<string> GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName)
        };

        // Set user role claims
        var roles = await userManager.GetRolesAsync(user);
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
