using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Instance;

public record InstanceInfo(
    InstanceId Id,
    ProjectId ProjectId,
    string Url,
    DateTimeOffset CreationDate
);
