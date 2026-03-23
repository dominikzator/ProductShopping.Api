using Microsoft.Extensions.DependencyInjection;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.MappingProfiles;
using ProductShopping.Application.Services;

namespace ProductShopping.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<ICartItemsService, CartItemsService>();
        services.AddScoped<IOrdersService, OrdersService>();

        services.AddAutoMapper(config => { }, typeof(ProductMappingProfile).Assembly);

        return services;
    }
}
