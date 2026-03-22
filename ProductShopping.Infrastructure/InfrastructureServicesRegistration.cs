using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ProductShopping.Infrastructure.Contracts;
using ProductShopping.Infrastructure.Services;

namespace ProductShopping.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped<IJWTService, JWTService>();

        return services;
    }
}
