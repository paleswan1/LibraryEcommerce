using LibraryEcom.Application.Common.Service;

namespace LibraryEcom.Helper.Implementation.Manager;

public class TokenManager: ISingletonService
{
    public readonly HashSet<string> BlackList = new();
}