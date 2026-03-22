using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Auth;
using ProductShopping.Api.Results;
using ProductShopping.Application.Models.Identity;
using ProductShopping.Domain.Models;
using ProductShopping.Identity.Constants;
using ProductShopping.Identity.Models;
using ProductShopping.Persistence.DatabaseContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductShopping.Api.Services;

public class UsersService(UserManager<ApplicationUser> userManager
    , IMailService mailService
    , IOptions<JwtSettings> jwtOptions
    , IConfiguration config
    , ProductShoppingDbContext productShoppingDbContext
    , IHttpContextAccessor httpContextAccessor
    , ILogger<UsersService> logger) : IUsersService
{
    public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            UserName = registerUserDto.Email,
            CreatedDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            logger.LogError("User registration failed for {Email}: {Errors}", registerUserDto.Email, string.Join(",", errors));

            return Result<RegisteredUserDto>.BadRequest(errors);
        }

        await userManager.AddToRoleAsync(user, RoleNames.User);

        var registeredUser = new RegisteredUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id
        };

        if (!productShoppingDbContext.Carts.Any(cart => cart.UserId == registeredUser.Id))
        {
            var userCart = new Cart
            {
                UserId = registeredUser.Id,
                //User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            productShoppingDbContext.Carts.Add(userCart);

            await productShoppingDbContext.SaveChangesAsync();
        }

        var emailConfirmed = await userManager.IsEmailConfirmedAsync(user);

        if(!emailConfirmed)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user); // [web:582][web:601]

            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenBytes); // [web:597]

            var confirmationUrl = $"{config["Constants:DomainName"]}/api/auth/confirm-email" +
                                  $"?userId={user.Id}&code={codeEncoded}";

            await mailService.SendEmailAsync(
                user.Email!,
                "Potwierdź swój e-mail",
                $"Kliknij, aby potwierdzić konto: {confirmationUrl}");
        }

        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            logger.LogWarning("Failed login attempt for email: {Email}", dto.Email);
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));
        }

        var valid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));
        }

        //Issue a token
        var token = await GenerateToken(user);


        return Result<string>.Success(token);
    }

    public string UserId => httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? string.Empty;

    private async Task<string> GenerateToken(ApplicationUser user)
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
