using ScriptBee.Ports.Notifications.Events;

namespace ScriptBee.Adapters.Notifications.SignalR.Contracts;

public record SignalRScriptUpdatedEvent(string ProjectId, string ScriptId, string ClientId = "")
{
    public static SignalRScriptUpdatedEvent Map(ScriptUpdatedEvent @event, string clientId)
    {
        return new SignalRScriptUpdatedEvent(
            @event.ProjectId.ToString(),
            @event.ScriptId.ToString(),
            clientId
        );
    }
}
