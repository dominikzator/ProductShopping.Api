using ProductShopping.Api.DTOs.Auth;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Contracts
{
    public interface IUsersService
    {
        Task<Result<string>> LoginAsync(LoginUserDto dto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);

        string UserId { get; }
    }
}