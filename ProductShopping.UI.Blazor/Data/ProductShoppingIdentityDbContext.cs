using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ProductShopping.Identity.Models;

namespace ProductShopping.Identity.DbContext
{
    public class ProductShoppingIdentityDbContext(DbContextOptions<ProductShoppingIdentityDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
    }
}
