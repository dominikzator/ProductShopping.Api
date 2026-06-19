namespace ProductShopping.Application.Contracts;

public interface IApplicationUserService
{
    Task<bool> IsEmailConfirmedAsync(string userId);
}
