using LibraryEcom.Application.Common.Service;

namespace LibraryEcom.Application.Interfaces.Services;

public interface INotificationsClient : IScopedService
{
    Task ReceiveNotification(string message);
    
    Task OrderPlaced(string userName, List<string> bookTitles);
}