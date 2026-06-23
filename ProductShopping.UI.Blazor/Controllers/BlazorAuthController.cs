using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.DTOs.Auth;
using ProductShopping.Identity.Models;

namespace ProductShopping.UI.Blazor.Controllers;

[ApiController]
[Route("api/blazor-auth")]
public class BlazorAuthController : ControllerBase
{
    private readonly IUsersService usersService;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<BlazorAuthController> logger;

    public BlazorAuthController(
        IUsersService usersService,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<BlazorAuthController> logger)
    {
        this.usersService = usersService;
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        logger.LogInformation("Blazor cookie login endpoint hit. Email={Email}", dto?.Email);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var validationResult = await usersService.ValidateCredentialsAsync(dto);
        if (!validationResult.IsSuccess)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var user = validationResult.Value;

        await signInManager.SignOutAsync();

        var result = await signInManager.PasswordSignInAsync(
            user.UserName!,
            dto.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            logger.LogWarning("Cookie login failed for {Email}", dto.Email);
            return Unauthorized("Invalid credentials.");
        }

        logger.LogInformation("Cookie login succeeded for {Email}", dto.Email);
        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}
