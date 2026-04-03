using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProductShopping.Api.EndToEndTests.Authentication;

public static class TestJwtTokenProvider
{
    public const string Issuer = "ProductShoppingTestIssuer";
    public const string Audience = "ProductShoppingTestAudience";
    public const string SecurityKey = "super-secret-test-key-123456789012345";

    public static string GenerateToken(
        string userId = "1",
        string email = "test@user.com",
        string role = "User")
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}