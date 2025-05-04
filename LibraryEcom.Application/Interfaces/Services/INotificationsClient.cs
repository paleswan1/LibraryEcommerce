namespace LibraryEcom.Application.Interfaces.Services;

public interface INotificationsClient
{
    Task ReceiveNotification(string message);
    
    Task OrderPlaced(string userName, List<string> bookTitles);
}