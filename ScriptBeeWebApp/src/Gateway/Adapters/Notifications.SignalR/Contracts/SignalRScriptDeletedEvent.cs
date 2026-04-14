using ScriptBee.Ports.Notifications.Events;

namespace ScriptBee.Adapters.Notifications.SignalR.Contracts;

public record SignalRScriptDeletedEvent(string ProjectId, string ScriptId, string ClientId = "")
{
    public static SignalRScriptDeletedEvent Map(ScriptDeletedEvent @event, string clientId)
    {
        return new SignalRScriptDeletedEvent(
            @event.ProjectId.ToString(),
            @event.ScriptId.ToString(),
            clientId
        );
    }
}
