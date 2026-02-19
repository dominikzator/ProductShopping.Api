using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Auth;
using ProductShopping.Api.Models;
using System.Text;

namespace ProductShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IUsersService usersService, UserManager<ApplicationUser> userManager, ILogger<AuthController> logger) : BaseApiController
    {
        /// <summary>
        /// Registers a user with given credentials. Can be called without authorization.
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var result = await usersService.RegisterAsync(registerUserDto);

            return ToActionResult(result);
        }

        /// <summary>
        /// Performs login operation with given credentials. Can be called without authorization.
        /// </summary>
        /// <param name="loginUserDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
        {
            var result = await usersService.LoginAsync(loginUserDto);

            return ToActionResult(result);
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            logger.LogInformation("Entered Anonymous ConfirmEmail Endpoint!");

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return BadRequest("Invalid data");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            // dekodowanie tokena
            var decodedBytes = WebEncoders.Base64UrlDecode(code);   // [web:597]
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);

            var result = await userManager.ConfirmEmailAsync(user, decodedToken); // ustawia EmailConfirmed = true [web:582][web:592]

            if (!result.Succeeded)
                return BadRequest("Token incorrect or expired");

            // w API możesz zwrócić np. prostą odpowiedź albo 302 do frontu
            return Ok("E-mail Confirmed.");
        }
    }
}
