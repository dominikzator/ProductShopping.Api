using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts;

public interface IIdentityUserService
{
    Task<bool> IsEmailConfirmedAsync(string userId);
    Task<string> GetUserIdFromOrderAsync(Order order); 
}
