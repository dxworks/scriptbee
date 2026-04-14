using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Adapters.Notifications.SignalR.Services;
using ScriptBee.Ports.Notifications;

namespace ScriptBee.Adapters.Notifications.SignalR;

public static class NotificationExtensions
{
    public static IServiceCollection AddProjectLiveUpdates(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IProjectNotificationsService, ProjectNotificationsService>();
        return services;
    }
}
