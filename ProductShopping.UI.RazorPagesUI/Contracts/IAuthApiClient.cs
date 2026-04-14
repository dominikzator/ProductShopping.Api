using ProductShopping.Application.DTOs.Auth;

namespace ProductShopping.UI.RazorPagesUI.Contracts
{
    public interface IAuthApiClient
    {
        Task<string> Login(LoginUserDto loginUserDto, CancellationToken ct = default);
        Task<RegisteredUserDto> Register(RegisterUserDto registerUserDto, CancellationToken ct = default);
    }
}