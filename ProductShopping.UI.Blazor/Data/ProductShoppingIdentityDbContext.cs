using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Identity.Models;

namespace ProductShopping.Identity.DbContext
{
    public class ProductShoppingIdentityDbContext(DbContextOptions<ProductShoppingIdentityDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
    }
}
