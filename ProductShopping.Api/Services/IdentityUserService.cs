using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Contracts;
using ProductShopping.Domain.Models;
using ProductShopping.Identity.DbContext;
using ProductShopping.Identity.Models;
using Stripe.Climate;

namespace ProductShopping.Api.Services;

public class IdentityUserService(ProductShoppingIdentityDbContext context, UserManager<ApplicationUser> userManager) : IIdentityUserService
{
    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        return await userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<string> GetUserIdFromOrderAsync(Domain.Models.Order order) => context.Users.FirstOrDefaultAsync(u => u.Id == order.CustomerId).Id.ToString();
}
