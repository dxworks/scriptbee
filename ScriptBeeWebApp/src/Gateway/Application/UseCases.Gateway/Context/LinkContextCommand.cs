using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Context;

public record LinkContextCommand(
    ProjectId ProjectId,
    InstanceId InstanceId,
    IEnumerable<string> LinkerIds
);
