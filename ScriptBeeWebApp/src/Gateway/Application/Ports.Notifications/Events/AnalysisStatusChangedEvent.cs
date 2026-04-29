using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Notifications.Events;

public record AnalysisStatusChangedEvent(ProjectId ProjectId, AnalysisId AnalysisId, string Status);
