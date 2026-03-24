using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Identity.Models;

namespace ProductShopping.Identity.DbContext;

public class ProductShoppingIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public ProductShoppingIdentityDbContext(DbContextOptions<ProductShoppingIdentityDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ProductShoppingIdentityDbContext).Assembly);
    }
}
