using Microsoft.Extensions.DependencyInjection;
using ProductShopping.Application.MappingProfiles;
using System.Reflection;

namespace ProductShopping.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(config => { }, typeof(ProductMappingProfile).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
