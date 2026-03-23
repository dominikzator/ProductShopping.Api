using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.DTOs.Auth;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using ProductShopping.Identity.Constants;
using ProductShopping.Identity.Models;
using ProductShopping.Persistence.DatabaseContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductShopping.Application.Services;

public class UsersService(UserManager<ApplicationUser> userManager
    , IJWTService jWTService
    , IMailService mailService
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
        var token = await jWTService.GenerateToken(new Application.DTOs.UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
        });


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
}
