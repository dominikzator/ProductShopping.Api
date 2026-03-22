using ProductShopping.Domain.Models;

namespace ProductShopping.Api.Contracts;

public interface IIdentityUserService
{
    Task<bool> IsEmailConfirmedAsync(string userId);
    Task<string> GetUserIdFromOrderAsync(Order order); 
}
