using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Persistence.DatabaseContext;
using ProductShopping.Persistence.Repositories;

namespace ProductShopping.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("ProductShoppingDbConnectionString");
        services.AddDbContext<ProductShoppingDbContext>(options =>
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
/*            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }*/

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<ICartsRepository, CartsRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}
