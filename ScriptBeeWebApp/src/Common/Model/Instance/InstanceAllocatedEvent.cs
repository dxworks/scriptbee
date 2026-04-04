using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Instance;

public sealed record InstanceAllocatedEvent(
    ProjectDetails projectDetails,
    InstanceInfo instanceInfo
);
