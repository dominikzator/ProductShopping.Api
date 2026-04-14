using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Identity.DbContext;
using ProductShopping.Identity.Models;
using ProductShopping.UI.RazorPagesUI.Clients;
using ProductShopping.UI.RazorPagesUI.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IProductsApiClient, ProductsApiClient>();
builder.Services.AddHttpClient<IProductsApiClient, ProductsApiClient>(client =>
{
    var apiUri = builder.Configuration["Api:BaseUrl"]!;
    client.BaseAddress = new Uri(apiUri);
});

builder.Services.AddScoped<ICartsApiClient, CartsApiClient>();
builder.Services.AddHttpClient<ICartsApiClient, CartsApiClient>(client =>
{
    var apiUri = builder.Configuration["Api:BaseUrl"]!;
    client.BaseAddress = new Uri(apiUri);
});

builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    var apiUri = builder.Configuration["Api:BaseUrl"]!;
    client.BaseAddress = new Uri(apiUri);
});

builder.Services
    .AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AddPageRoute("/Products", "");
    });

var connectionString = builder.Configuration.GetConnectionString("ProductShoppingDbConnectionString");

builder.Services.AddDbContext<ProductShoppingIdentityDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ProductShoppingIdentityDbContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
