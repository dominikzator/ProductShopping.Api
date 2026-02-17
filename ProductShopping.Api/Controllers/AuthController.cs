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
    }
}
