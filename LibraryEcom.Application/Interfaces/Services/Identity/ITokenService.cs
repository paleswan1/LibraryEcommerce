using LibraryEcom.Application.Common.Service;

namespace LibraryEcom.Application.Interfaces.Services.Identity;

public interface ITokenService : ITransientService
{
    Task<bool> IsCurrentActiveToken();
    
    Task DeactivateCurrentAsync();
    
    Task<bool> IsActiveAsync(string token);
    
    Task DeactivateAsync(string token);
}