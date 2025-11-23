using System.Text;
using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.Common;
using BusinessReportsManager.Application.Validation;
using BusinessReportsManager.Domain.Interfaces;
using BusinessReportsManager.Infrastructure.DataAccess;
using BusinessReportsManager.Infrastructure.DataAccess.Seeders;
using BusinessReportsManager.Infrastructure.Identity;
using BusinessReportsManager.Infrastructure.Security;
using BusinessReportsManager.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

// ✅ Force Development environment (for local debugging, e.g., Rider)

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
var builder = WebApplication.CreateBuilder(args);
builder.Environment.EnvironmentName = "Development";

// -------------------- Services Configuration --------------------

// Controllers + Filters
builder.Services.AddControllers(options =>
{
    // options.Filters.Add<ConcurrencyExceptionFilter>();
}).AddNewtonsoftJson();

builder.Services.AddProblemDetails();

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
    );
});

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? "THIS_IS_A_DEMO_SECRET_CHANGE_ME");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"] ?? "BusinessReportsManager",
        ValidAudience = jwtSection["Audience"] ?? "BusinessReportsManagerAudience",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.CanViewAllOrders, p => p.RequireRole("Accountant", "Supervisor"));
    options.AddPolicy(AppPolicies.CanEditAllOrders, p => p.RequireRole("Accountant", "Supervisor"));
    options.AddPolicy(AppPolicies.CanEditOwnOpenOrders, p => p.RequireRole("Employee"));
});

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderDtoValidator>();

// ✅ AutoMapper (fixes IMapper dependency issues)
builder.Services.AddAutoMapper(cfg => { /* optional config here */ }, typeof(Program).Assembly);

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BusinessReportsManager API",
        Version = "v1",
        Description = "API documentation for Business Reports Manager system"
    });

    // Include XML comments (optional)
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
    foreach (var xml in xmlFiles)
        c.IncludeXmlComments(xml, includeControllerXmlComments: true);

    // Add example filters
    c.ExampleFilters();

    // JWT Security Scheme
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        Array.Empty<string>()
    }
});
});

// Swagger example providers
builder.Services.AddSwaggerExamplesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

// HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Application Services
builder.Services.AddScoped<IGenericRepository, GenericRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IOrderPartyService, OrderPartyService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<BusinessReportsManager.Domain.Interfaces.IOrderNumberGenerator, OrderNumberGenerator>();

// -------------------- App Pipeline --------------------
var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BusinessReportsManager API v1");
        c.RoutePrefix = string.Empty; // Swagger at root URL
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BusinessReportsManager API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Database migration + seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await AppDbSeeder.SeedAsync(db, userManager, roleManager);
}

app.Run();
