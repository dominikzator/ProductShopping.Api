using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Identity.Models;

namespace ProductShopping.Identity.DbContext;

public class ProductShoppingIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public ProductShoppingIdentityDbContext(DbContextOptions<ProductShoppingIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
