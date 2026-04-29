using ScriptBee.Ports.Notifications.Events;

namespace ScriptBee.Adapters.Notifications.SignalR.Contracts;

public record SignalRAnalysisStatusChangedEvent(string ProjectId, string AnalysisId, string Status)
{
    public static SignalRAnalysisStatusChangedEvent Map(AnalysisStatusChangedEvent @event)
    {
        return new SignalRAnalysisStatusChangedEvent(
            @event.ProjectId.ToString(),
            @event.AnalysisId.ToString(),
            @event.Status
        );
    }
}
