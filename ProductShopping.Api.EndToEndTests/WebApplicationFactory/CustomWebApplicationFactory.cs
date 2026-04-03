using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using ProductShopping.Api.EndToEndTests.Authentication;
using ProductShopping.Api.EndToEndTests.Handlers;
using ProductShopping.Api.EndToEndTests.Seeders;
using ProductShopping.Identity.DbContext;
using ProductShopping.Persistence.DatabaseContext;
using System.Security.Claims;
using System.Text;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ProductShoppingDbContext>>();
            services.RemoveAll<ProductShoppingDbContext>();

            services.RemoveAll<DbContextOptions<ProductShoppingIdentityDbContext>>();
            services.RemoveAll<ProductShoppingIdentityDbContext>();

            services.AddDbContext<ProductShoppingDbContext>(options =>
            {
                options.UseInMemoryDatabase("ProductShoppingApiTests");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            services.AddDbContext<ProductShoppingIdentityDbContext>(options =>
            {
                options.UseInMemoryDatabase("ProductShoppingIdentityTests");
            });

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            services.PostConfigureAll<JwtBearerOptions>(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = TestJwtTokenProvider.Issuer,

                    ValidateAudience = true,
                    ValidAudience = TestJwtTokenProvider.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(TestJwtTokenProvider.SecurityKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateActor = false,
                    ValidateTokenReplay = false,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var persistenceDb = scope.ServiceProvider.GetRequiredService<ProductShoppingDbContext>();
            var identityDb = scope.ServiceProvider.GetRequiredService<ProductShoppingIdentityDbContext>();

            persistenceDb.Database.EnsureDeleted();
            persistenceDb.Database.EnsureCreated();

            identityDb.Database.EnsureDeleted();
            identityDb.Database.EnsureCreated();

            TestDataSeeder.Seed(persistenceDb).GetAwaiter().GetResult();
        });
    }
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var appContext = scope.ServiceProvider.GetRequiredService<ProductShoppingDbContext>();
        var identityContext = scope.ServiceProvider.GetRequiredService<ProductShoppingIdentityDbContext>();

        await appContext.Database.EnsureDeletedAsync();
        await appContext.Database.EnsureCreatedAsync();

        await identityContext.Database.EnsureDeletedAsync();
        await identityContext.Database.EnsureCreatedAsync();

        await TestDataSeeder.Seed(appContext);
    }
}