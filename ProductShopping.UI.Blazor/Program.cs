using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductShopping.Api.Contracts;
using ProductShopping.Application;
using ProductShopping.Application.Services;
using ProductShopping.Identity;
using ProductShopping.Identity.DbContext;
using ProductShopping.Identity.Models;
using ProductShopping.Infrastructure;
using ProductShopping.Persistence;
using ProductShopping.UI.Blazor.Components;
using ProductShopping.UI.Blazor.Components.Account;
using ProductShopping.UI.Blazor.Contracts;
using ProductShopping.UI.Blazor.Services.Auth;
using ProductShopping.UI.Shared.Clients;
using ProductShopping.UI.Shared.Contracts;
using ProductShopping.UI.Shared.Services.Auth;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ProductShoppingIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ProductShoppingIdentityDbContextConnection' not found.");;

builder.Services.AddDbContext<ProductShoppingIdentityDbContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<IProductsApiClient, ProductsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
});

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Host:BaseUrl"]!);
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<ApplicationUserAccessor>();

builder.Services.AddScoped<IdentityRedirectManager>();

/*builder.Services.AddInfrastructureServices(builder);
builder.Services.AddPersistenceServices(builder);*/

builder.Services.AddInfrastructureServices(builder);
builder.Services.AddPersistenceServices(builder);
//builder.Services.AddIdentityServices(builder);
builder.Services.AddApplicationServices();

//builder.Services.AddScoped<IUsersService, UsersService>();

/*builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddScoped<ITokenStorage, ProtectedSessionTokenStorage>();
builder.Services.AddScoped<IJwtClaimsFactory, JwtClaimsFactory>();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());*/

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ProductShoppingIdentityDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();;

app.Run();
