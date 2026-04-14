using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.Auth;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using ProductShopping.Identity.Constants;
using ProductShopping.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductShopping.Application.Services;

public class UsersService(ICartsRepository cartsRepository
    , UserManager<ApplicationUser> userManager
    , IJWTService jWTService
    , IMailService mailService
    , IConfiguration config
    , IHttpContextAccessor httpContextAccessor
    , IAppLogger<UsersService> logger) : IUsersService
{
    public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
    {
        Console.WriteLine("RegisterAsync API");

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

        var userCart = new Cart
        {
            UserId = registeredUser.Id,
        };
        await cartsRepository.CreateAsync(userCart);

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
                "Confirm your e-mail",
                $"Click to confirm e-mail: {confirmationUrl}");
        }

        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto dto)
    {
        Console.WriteLine("LoginAsync API");
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
        var token = await jWTService.GenerateToken(new DTOs.UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
        });


        return Result<string>.Success(token);
    }

/*    public async Task<Result<string>> LogoutAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "HttpContext is not available."));
        }

        var user = httpContext.User;

        var jti = user.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                  ?? user.FindFirst("jti")?.Value;

        if (string.IsNullOrWhiteSpace(jti))
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Token JTI not found."));
        }

        var expClaim = user.FindFirst(JwtRegisteredClaimNames.Exp)?.Value
                       ?? user.FindFirst("exp")?.Value;

        DateTime expiresAtUtc = DateTime.UtcNow.AddHours(1);

        if (!string.IsNullOrWhiteSpace(expClaim) && long.TryParse(expClaim, out var expUnix))
        {
            expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        }

        await revokedTokenStore.RevokeAsync(jti, expiresAtUtc);

        logger.LogInformation("Token revoked. JTI: {Jti}, UserId: {UserId}, Email: {Email}",
            jti, GetUserId(), GetUserEmail());

        return Result<string>.Success("Logged out successfully.");
    }*/

    public string GetUserId() => httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? string.Empty;

    public string GetUserEmail() => httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(JwtRegisteredClaimNames.Email)?.Value!
        ?? httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.Email)?.Value!;
}
