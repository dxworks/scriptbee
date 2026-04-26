using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Instance;

public sealed record InstanceDeallocatedEvent(
    ProjectDetails projectDetails,
    InstanceInfo instanceInfo
);
