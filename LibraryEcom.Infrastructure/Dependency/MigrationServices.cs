using LibraryEcom.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryEcom.Infrastructure.Dependency;

public static class MigrationServices
{
    public static async Task AddDataSeedMigrationService(this IServiceCollection services)
    {
        await SeedDefaultDataSets(services);

        await SeedAdmin(services);

        await SeedServiceDataSets(services);
    }

    private static async Task SeedDefaultDataSets(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

       
    }
    
    private static async Task SeedAdmin(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        await dbInitializer.InitializeIdentityData();
    }
    
    private static async Task SeedServiceDataSets(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

      
    }
}