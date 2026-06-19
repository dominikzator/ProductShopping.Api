using Microsoft.AspNetCore.Identity;
using ProductShopping.Application.Contracts;
using ProductShopping.Identity.Models;

namespace ProductShopping.Infrastructure.Services;

public class ApplicationUserService(UserManager<ApplicationUser> userManager) : IApplicationUserService
{
    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return await userManager.IsEmailConfirmedAsync(user!);
    }
}
