using ProductShopping.Application.DTOs.Auth;
using ProductShopping.Application.Results;

namespace ProductShopping.Api.Contracts
{
    public interface IUsersService
    {
        Task<Result<string>> LoginAsync(LoginUserDto dto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);

        string UserId { get; }
    }
}