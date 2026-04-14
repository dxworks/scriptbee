using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Notifications.Events;

public record ScriptUpdatedEvent(ProjectId ProjectId, ScriptId ScriptId, string ClientId = "");
