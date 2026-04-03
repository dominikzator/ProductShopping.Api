using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Domain.Common;
using ProductShopping.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace ProductShopping.Persistence.DatabaseContext;

public class ProductShoppingDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductShoppingDbContext(DbContextOptions<ProductShoppingDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem > CartItems { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public Task<List<ProductCategory>> GetProductCategoriesAsNoTracking() => ProductCategories.AsNoTracking().ToListAsync();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<Order>().OwnsOne(typeof(Address), "Address");

        builder.Entity<ProductCategory>()
            .HasIndex(c => c.Name)
            .HasDatabaseName("IX_Countries_Name");

        builder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        builder.Entity<Cart>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        builder.Entity<CartItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        builder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId);

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
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in base.ChangeTracker.Entries<BaseEntity>().Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
        {
            entry.Entity.DateModified = DateTime.UtcNow;
            entry.Entity.ModifiedBy = GetUserId();
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DateCreated = DateTime.UtcNow;
                entry.Entity.CreatedBy = GetUserId();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public string GetUserId() => _httpContextAccessor?
        .HttpContext?
        .User?
        .FindFirst(JwtRegisteredClaimNames.Sub)?.Value
    ?? _httpContextAccessor?
        .HttpContext?
        .User?
        .FindFirst(ClaimTypes.NameIdentifier)?.Value
    ?? string.Empty;
}
