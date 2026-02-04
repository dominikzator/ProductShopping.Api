using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Models;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Reflection.Emit;

namespace ProductShopping.Api;

public class ProductShoppingDbContext : IdentityDbContext<ApplicationUser>
{
    public ProductShoppingDbContext(DbContextOptions<ProductShoppingDbContext> options) : base(options)
    {

    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<ProductCategory>()
            .HasIndex(c => c.Name)
            .HasDatabaseName("IX_Countries_Name");

        builder.Entity<ProductCategory>().HasData(
            new ProductCategory { Id = 1, Name = "Food" },
            new ProductCategory { Id = 2, Name = "Garden" },
            new ProductCategory { Id = 3, Name = "Automotive" },
            new ProductCategory { Id = 4, Name = "Health" },
            new ProductCategory { Id = 5, Name = "Kitchen" },
            new ProductCategory { Id = 6, Name = "Office" },
            new ProductCategory { Id = 7, Name = "Clothing" },
            new ProductCategory { Id = 8, Name = "Home" },
            new ProductCategory { Id = 9, Name = "Outdoor" },
            new ProductCategory { Id = 10, Name = "Fitness" },
            new ProductCategory { Id = 11, Name = "Audio" },
            new ProductCategory { Id = 12, Name = "Electronics" },
            new ProductCategory { Id = 13, Name = "Home Improvement" },
            new ProductCategory { Id = 14, Name = "Pets" },
            new ProductCategory { Id = 15, Name = "Travel" },
            new ProductCategory { Id = 16, Name = "Toys" },
            new ProductCategory { Id = 17, Name = "Wearable Tech" },
            new ProductCategory { Id = 18, Name = "Crafts" },
            new ProductCategory { Id = 19, Name = "Music" },
            new ProductCategory { Id = 20, Name = "Bicycles" },
            new ProductCategory { Id = 21, Name = "Furniture" },
            new ProductCategory { Id = 22, Name = "Beauty" },
            new ProductCategory { Id = 23, Name = "Art Supplies" },
            new ProductCategory { Id = 24, Name = "Gaming" },
            new ProductCategory { Id = 25, Name = "Home Appliances" },
            new ProductCategory { Id = 26, Name = "Smart Home" },
            new ProductCategory { Id = 27, Name = "Books" },
            new ProductCategory { Id = 28, Name = "Photography" },
            new ProductCategory { Id = 29, Name = "Sports" },
            new ProductCategory { Id = 30, Name = "Baby" },
            new ProductCategory { Id = 31, Name = "Tools" },
            new ProductCategory { Id = 32, Name = "Computers" },
            new ProductCategory { Id = 33, Name = "Safety" },
            new ProductCategory { Id = 34, Name = "Storage" },
            new ProductCategory { Id = 35, Name = "Home Security" },
            new ProductCategory { Id = 36, Name = "Accessories" }
        );
    }
}
