using Microsoft.Extensions.DependencyInjection;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.MappingProfiles;
using ProductShopping.Application.Services;
using System.Reflection;

namespace ProductShopping.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersService>();

        services.AddAutoMapper(config => { }, typeof(ProductMappingProfile).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
