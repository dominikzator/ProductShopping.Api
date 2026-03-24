using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Contracts;
using ProductShopping.Identity.Models;

namespace ProductShopping.Infrastructure.Services;

public class IdentityUserService(UserManager<ApplicationUser> userManager) : IIdentityUserService
{
    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return await userManager.IsEmailConfirmedAsync(user!);
    }
}
