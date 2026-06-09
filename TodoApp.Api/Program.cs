using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoApp.Api.Application.Services;
using TodoApp.Api.Core.Interfaces;
using TodoApp.Api.Infrastructure.Auth;
using TodoApp.Api.Infrastructure.Data;
using TodoApp.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    if (connectionString != null)
    {
        options.UseMySql(connectionString, ServerVersion.Parse("8.0.0-mysql"));
    }
});

// Configure DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TodoService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure Controllers
builder.Services.AddControllers();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "super_secret_key_change_me_in_production"))
        };
    });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
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
            new string[] {}
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TodoDbContext>();
        await DatabaseSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        // Log the error in a real app
        Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
        c.RoutePrefix = "swagger"; // So Swagger is at /swagger
    });
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
