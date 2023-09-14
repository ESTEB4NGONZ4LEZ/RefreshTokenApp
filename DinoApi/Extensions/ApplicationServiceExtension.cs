
using System.Text;
using Aplicacion.UnitOfWork;
using DinoApi.Helpers;
using DinoApi.Service;
using Dominio.Entities;
using Dominio.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DinoApi.Extensions;

public static class ApplicationServiceExtension
{
    public static void AddConfigureCors(this IServiceCollection services) => services.AddCors(opt => {
        opt.AddPolicy("CorsPolicy", builder => 
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowAnyOrigin();
        });
    });

    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }

    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Jwt>(configuration.GetSection("Jwt"));
        services.AddAuthentication(opt => 
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt => 
        {
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });
        services.AddAuthorization();
    }
}
