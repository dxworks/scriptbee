using ScriptBee.Ports.Notifications.Events;

namespace ScriptBee.Ports.Notifications;

public interface IProjectNotificationsService
{
    Task NotifyScriptCreated(ScriptCreatedEvent scriptCreatedEvent, CancellationToken cancellationToken);

    Task NotifyScriptUpdated(ScriptUpdatedEvent scriptUpdatedEvent, CancellationToken cancellationToken);

    Task NotifyScriptDeleted(ScriptDeletedEvent scriptDeletedEvent, CancellationToken cancellationToken);
}
