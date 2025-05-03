using LibraryEcom.Application.Interfaces.Data;
using LibraryEcom.Application.Settings;
using LibraryEcom.Domain.Common.Property;
using LibraryEcom.Helper;
using LibraryEcom.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryEcom.Infrastructure.Dependency;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = new DatabaseSettings();

        configuration.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);

        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseDatabase(databaseSettings.DbProvider, connectionString!);
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetService<ApplicationDbContext>()!);

        services.AddHttpClient();

        services.AddDistributedMemoryCache();

        EnsureDatabaseMigrated(services);
        
        services.EnableCors(configuration);

        return services;
    }

    private static void EnsureDatabaseMigrated(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
    
    private static void EnableCors(this IServiceCollection services, IConfiguration configuration)
    {
        var clientSettings = new ClientSettings();

        configuration.GetSection(nameof(ClientSettings)).Bind(clientSettings);

        var baseUrls = clientSettings.BaseUrl.Split(";");

        foreach (var baseUrl in baseUrls)
        {
            Console.WriteLine($"CORS Allowed Environment: {baseUrl}.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy(name: Constants.Cors.MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(baseUrls)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
    }
}