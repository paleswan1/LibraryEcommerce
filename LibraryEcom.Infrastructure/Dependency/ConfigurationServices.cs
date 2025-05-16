using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace LibraryEcom.Infrastructure.Dependency;

public static class ConfigurationServices
{
    public static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        var environment = builder.Environment;
        
        var configuration = builder.Configuration;
        
        Console.WriteLine($"Environment: {environment.EnvironmentName}.");

        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        return builder;
    }
}