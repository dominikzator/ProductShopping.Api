using ProductShopping.Application.DTOs.Auth;
using ProductShopping.Application.Models.Identity;
using ProductShopping.Application.Results;

namespace ProductShopping.Api.Contracts
{
    public interface IUsersService
    {
        Task<Result<string>> LoginAsync(LoginUserDto dto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);

        string GetUserId();
        string GetUserEmail();
        Task<Result<AuthenticatedUser>> ValidateCredentialsAsync(LoginUserDto dto);
    }
}