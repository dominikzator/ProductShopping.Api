using ProductShopping.Application.DTOs.Auth;

namespace ProductShopping.UI.Shared.Contracts;

public interface IAuthApiClient
{
    Task<string> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task<RegisteredUserDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
}