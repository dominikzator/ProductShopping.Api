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

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem > CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<ProductCategory>()
            .HasIndex(c => c.Name)
            .HasDatabaseName("IX_Countries_Name");

        builder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        builder.Entity<Cart>(entity =>
        {
            entity.Property(e => e.CartId).ValueGeneratedOnAdd();
        });
        builder.Entity<CartItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<ProductCategory>().HasData(
            new { Id = 1, Name = "Food" },
            new { Id = 2, Name = "Garden" },
            new { Id = 3, Name = "Automotive" },
            new { Id = 4, Name = "Health" },
            new { Id = 5, Name = "Kitchen" },
            new { Id = 6, Name = "Office" },
            new { Id = 7, Name = "Clothing" },
            new { Id = 8, Name = "Home" },
            new { Id = 9, Name = "Outdoor" },
            new { Id = 10, Name = "Fitness" },
            new { Id = 11, Name = "Audio" },
            new { Id = 12, Name = "Electronics" },
            new { Id = 13, Name = "Home Improvement" },
            new { Id = 14, Name = "Pets" },
            new { Id = 15, Name = "Travel" },
            new { Id = 16, Name = "Toys" },
            new { Id = 17, Name = "Wearable Tech" },
            new { Id = 18, Name = "Crafts" },
            new { Id = 19, Name = "Music" },
            new { Id = 20, Name = "Bicycles" },
            new { Id = 21, Name = "Furniture" },
            new { Id = 22, Name = "Beauty" },
            new { Id = 23, Name = "Art Supplies" },
            new { Id = 24, Name = "Gaming" },
            new { Id = 25, Name = "Home Appliances" },
            new { Id = 26, Name = "Smart Home" },
            new { Id = 27, Name = "Books" },
            new { Id = 28, Name = "Photography" },
            new { Id = 29, Name = "Sports" },
            new { Id = 30, Name = "Baby" },
            new { Id = 31, Name = "Tools" },
            new { Id = 32, Name = "Computers" },
            new { Id = 33, Name = "Safety" },
            new { Id = 34, Name = "Storage" },
            new { Id = 35, Name = "Home Security" },
            new { Id = 36, Name = "Accessories" }
        );
    }
}
