using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application;
using ProductShopping.Identity.DbContext;
using ProductShopping.Identity.Models;
using ProductShopping.Infrastructure;
using ProductShopping.Persistence;
using ProductShopping.UI.Blazor.Authentication;
using ProductShopping.UI.Blazor.Components;
using ProductShopping.UI.Blazor.Components.Account;
using ProductShopping.UI.Blazor.Contracts;
using ProductShopping.UI.Blazor.Services.Auth;
using ProductShopping.UI.Shared.Clients;
using ProductShopping.UI.Shared.Contracts;
using ProductShopping.UI.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ProductShoppingIdentityDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ProductShoppingIdentityDbContextConnection' not found.");

builder.Services.AddDbContext<ProductShoppingIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddHttpClient<IProductsApiClient, ProductsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
});

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<ITokenStore, ProtectedSessionTokenStorage>();

builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddInfrastructureServices(builder);
builder.Services.AddPersistenceServices(builder);
builder.Services.AddApplicationServices();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ProductShoppingIdentityDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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

app.MapAdditionalIdentityEndpoints();

app.Run();