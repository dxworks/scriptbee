using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Analysis;

public record InstanceInfo(
    InstanceId Id,
    ProjectId ProjectId,
    string Url,
    DateTimeOffset CreationDate
);
