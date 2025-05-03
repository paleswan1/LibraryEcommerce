using LibraryEcom.Application.Common.Service;

namespace LibraryEcom.Infrastructure.Persistence.Seed;

public interface IDbInitializer : IScopedService
{
    Task InitializeIdentityData(CancellationToken cancellationToken = default);

  
}