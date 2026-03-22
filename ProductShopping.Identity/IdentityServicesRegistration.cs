using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductShopping.Identity.DbContext;
using ProductShopping.Identity.Models;

namespace ProductShopping.Identity;

public static class IdentityServicesRegistration
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("ProductShoppingDbConnectionString");
        services.AddDbContext<ProductShoppingIdentityDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(30);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null
                );
            });
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddIdentityApiEndpoints<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ProductShoppingIdentityDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
