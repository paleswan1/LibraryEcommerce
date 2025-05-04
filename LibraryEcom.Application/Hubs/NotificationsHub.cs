using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace LibraryEcom.Application.Hubs;

public class NotificationsHub: Hub<INotificationsClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        Console.WriteLine($"User connected: {userId}");
        await base.OnConnectedAsync();
    }
}

