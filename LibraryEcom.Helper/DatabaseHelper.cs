using LibraryEcom.Domain.Common.Property;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcom.Helper;

public static class DatabaseHelper
{
    public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
    {
        return dbProvider.ToLowerInvariant() switch
        {
            Constants.DbProviderKeys.Npgsql => builder.UseNpgsql(connectionString, e =>
                e.MigrationsAssembly("LibraryEcom.Migrators.PostgreSQL")),

            _ => throw new NotSupportedException($"Database provider '{dbProvider}' is not supported.")
        };
    }
}