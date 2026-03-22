using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Domain.Models;
using ProductShopping.Identity.Models;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Reflection.Emit;

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
