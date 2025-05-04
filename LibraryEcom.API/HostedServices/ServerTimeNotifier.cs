using LibraryEcom.Application.Hubs;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace LibraryCom.HostedServices;

public class ServerTimeNotifier:BackgroundService
{
    private static readonly TimeSpan Period = TimeSpan.FromSeconds(20);
    private readonly ILogger<ServerTimeNotifier> _logger;
    private readonly IHubContext<NotificationsHub,INotificationsClient> _hubContext;

    public ServerTimeNotifier(ILogger<ServerTimeNotifier> logger, IHubContext<NotificationsHub, INotificationsClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Period);

        while (!stoppingToken.IsCancellationRequested &&
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            var dateTime = DateTime.UtcNow;
            
            _logger.LogInformation("Executing {ServiceName}{Time}", nameof(ServerTimeNotifier),dateTime);
            
            _hubContext.Clients.All.ReceiveNotification($"ServerTimeNotifier: {dateTime}");
        }
    }
}