using ProductShopping.Application.DTOs.Auth;

namespace ProductShopping.UI.Shared.Contracts
{
    public interface IAuthApiClient
    {
        Task LoginForBlazorAsync(LoginUserDto dto, CancellationToken cancellationToken);
        Task LogoutForBlazorAsync(CancellationToken cancellationToken);
    }
}