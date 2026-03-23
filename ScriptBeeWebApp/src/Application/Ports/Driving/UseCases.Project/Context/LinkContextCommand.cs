using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Context;

public record LinkContextCommand(
    ProjectId ProjectId,
    InstanceId InstanceId,
    IEnumerable<string> LinkerIds
);
