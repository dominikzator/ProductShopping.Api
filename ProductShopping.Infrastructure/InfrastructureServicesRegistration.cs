using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Services;
using ProductShopping.Infrastructure.Logging;
using ProductShopping.Infrastructure.Services;
using Stripe;

namespace ProductShopping.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IIdentityUserService, IdentityUserService>();
        services.AddScoped<IProductImageGeneratorService, ProductImageGeneratorService>();
        services.AddScoped<IPaymentsService, PaymentsService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

        builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

        Console.WriteLine("StripeConfiguration.ApiKey: " + StripeConfiguration.ApiKey);

        return services;
    }
}
