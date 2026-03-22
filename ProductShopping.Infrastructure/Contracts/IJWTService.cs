using ProductShopping.Identity.Models;

namespace ProductShopping.Infrastructure.Contracts;

public interface IJWTService
{
    Task<string> GenerateToken(ApplicationUser user);
}