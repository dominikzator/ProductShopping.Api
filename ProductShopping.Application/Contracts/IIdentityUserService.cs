namespace ProductShopping.Application.Contracts;

public interface IIdentityUserService
{
    Task<bool> IsEmailConfirmedAsync(string userId);
}
