using Microsoft.AspNetCore.SignalR;
using ScriptBee.Adapters.Notifications.SignalR.Hubs;
using ScriptBee.Application.Model.Services;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Notifications;
using ScriptBee.Ports.Notifications.Events;

namespace ScriptBee.Adapters.Notifications.SignalR.Services;

public class ProjectNotificationsService(
    IHubContext<ProjectLiveUpdatesHub> hubContext,
    IClientIdProvider clientIdProvider
) : IProjectNotificationsService
{
    private const string ScriptsChannel = "scripts";

    public Task NotifyScriptCreated(ScriptCreatedEvent scriptCreatedEvent, CancellationToken cancellationToken)
    {
        var groupName = GetGroupName(scriptCreatedEvent.ProjectId, ScriptsChannel);
        var message = scriptCreatedEvent with { ClientId = clientIdProvider.ClientId ?? "" };
        return hubContext.Clients.Group(groupName).SendAsync("ScriptCreated", message, cancellationToken);
    }

    public Task NotifyScriptUpdated(ScriptUpdatedEvent scriptUpdatedEvent, CancellationToken cancellationToken)
    {
        var groupName = GetGroupName(scriptUpdatedEvent.ProjectId, ScriptsChannel);
        var message = scriptUpdatedEvent with { ClientId = clientIdProvider.ClientId ?? "" };
        return hubContext.Clients.Group(groupName).SendAsync("ScriptUpdated", message, cancellationToken);
    }

    public Task NotifyScriptDeleted(ScriptDeletedEvent scriptDeletedEvent, CancellationToken cancellationToken)
    {
        var groupName = GetGroupName(scriptDeletedEvent.ProjectId, ScriptsChannel);
        var message = scriptDeletedEvent with { ClientId = clientIdProvider.ClientId ?? "" };
        return hubContext.Clients.Group(groupName).SendAsync("ScriptDeleted", message, cancellationToken);
    }

    private static string GetGroupName(ProjectId projectId, string channelName) => $"{projectId.Value}_{channelName}";
}
