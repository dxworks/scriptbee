using ScriptBee.Ports.Notifications.Events;

namespace ScriptBee.Adapters.Notifications.SignalR.Contracts;

public record SignalRScriptCreatedEvent(
    string ProjectId,
    string ScriptId,
    string? ParentId,
    string Path,
    string ClientId = ""
)
{
    public static SignalRScriptCreatedEvent Map(ScriptCreatedEvent @event, string clientId)
    {
        return new SignalRScriptCreatedEvent(
            @event.ProjectId.ToString(),
            @event.ScriptId.ToString(),
            @event.ParentId?.ToString(),
            @event.Path,
            clientId
        );
    }
}
