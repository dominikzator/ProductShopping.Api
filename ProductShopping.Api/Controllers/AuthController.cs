using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Auth;

namespace ProductShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IUsersService usersService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var result = await usersService.RegisterAsync(registerUserDto);

            return ToActionResult(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
        {
            var result = await usersService.LoginAsync(loginUserDto);

            return ToActionResult(result);
        }
    }
}
