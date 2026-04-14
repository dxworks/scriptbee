using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Notifications.Events;

public record ScriptCreatedEvent(ProjectId ProjectId, ScriptId ScriptId, string ClientId = "");
