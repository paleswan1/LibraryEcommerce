using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using LibraryEcom.Application.Settings;
using Microsoft.IdentityModel.Tokens;
using LibraryEcom.Application.Interfaces.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LibraryEcom.Domain.Entities.Identity;
using LibraryEcom.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using LibraryEcom.Domain.Common;
using LibraryEcom.Domain.Common.Property;
using LibraryEcom.Helper;
using LibraryEcom.Helper.Implementation.Manager;
using Microsoft.Extensions.Options;

namespace LibraryEcom.Identity.Dependency;

public static class IdentityServices
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = new DatabaseSettings();

        configuration.GetSection("DatabaseSettings").Bind(databaseSettings);

        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseDatabase(databaseSettings.DbProvider, connectionString!);
        });

        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);

        services.AddIdentity<User, Role>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddSingleton<TokenManager>();

        services.Configure<IdentityOptions>(options =>
            options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

        services.AddHttpContextAccessor();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.Audience = configuration["JwtSettings:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ?? string.Empty)),
                };
            });

        services.AddAuthorization();

        return services;
    }
}