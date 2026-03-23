using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductShopping.Application;
using ProductShopping.Application.Models.Identity;
using ProductShopping.Identity;
using ProductShopping.Identity.Models;
using ProductShopping.Infrastructure;
using ProductShopping.Persistence;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*Log.Information("Starting ProductShopping API");

builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
    );*/

// Add services to the container.

builder.Services.AddInfrastructureServices(builder);
builder.Services.AddIdentityServices(builder);
builder.Services.AddPersistenceServices(builder);
builder.Services.AddApplicationServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    //Log.Fatal("JwtSettings:Key is not configured");
    throw new InvalidOperationException("JwtSettings:Key is not configured");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });
//.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { })
//.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationDefaults.apiKeyScheme, _ => { }
//);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // API Information
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Product Shopping API",
        Description = "API for browsing, adding to cart and ordering products. <br /><br />"
		+ " Test flow: <br /> 🔐 Register a new User with Post register endpoint or Login with a testing User: email: testinguser@localhost.com, password: P@ssword1234.<br />"
        + "📧 If You will register with your real email, You will get a Confirmation Email and after creating an order and successfull testing payment You will get an order/payment confirmation email. <br />"
        + "🔐 After logging in You will get a JWT Token as a response. Copy it and place it in Authorize Section, value in a field should be: Bearer [space] [yourToken] and click Authorize. <br />"
        + "🛍️ You can browse Products with Filters and/or with PaginationParameters with Products Get endpoint. This endpoint doesn't need a token authorization. <br />"
        + "🛍️ Products in database have id range from 3009 to 4008. <br />"
        + "🛒 Now You can add some Products to Cart. You need to specify product ID and Amount. You can add/remove multiple Products to Cart.<br />"
        + "🛒 You can check current Cart content by Cart Get endpoint. <br />"
        + "📦 When Your Cart is not empty, You can make an Order with Order Post endpoint. You need to specify some Address informations. <br />"
        + "💳 After successfull order You will get a testing payment url as a response. Open it in a different browser card. <br />"
        + "💳 In payment url site You should see your products from Cart. <br />"
        + "💳 To realize testing payment enter card number: 4242 4242 4242 4242. The rest card credentials can be random. This is testing payment. You won't be charged for anything. <br />"
        + "🚀 This is it! You can check your Order with Get Order endpoint to check if your order changed its status from Pending to Payed. <br />"
        + "📧 If Your Account have confirmed Email You will also get an email with order confirmation!",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "dominikzator@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (System.IO.File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Enable annotations
    options.EnableAnnotations();

    // Security Definitions
    // JWT Bearer Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    // Add security requirements
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });

    // Add operation filters for examples
    options.ExampleFilters();

    // Custom operation filter for handling multiple auth schemes
    //options.OperationFilter<HotelListing.Api.Filters.SecurityRequirementsOperationFilter>();

    // Order actions by method
    options.OrderActionsBy(apiDesc => $"{apiDesc.RelativePath}_{apiDesc.HttpMethod}");
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsDev", policy =>
    {
        policy.WithOrigins("https://localhost:7277")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Shopping API V1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Product Shopping API Documentation";
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
    options.ShowExtensions();
    options.EnableValidator();
});

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
