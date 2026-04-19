using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Context;

public sealed record GenerateClassesCommand(
    ProjectId ProjectId,
    InstanceId InstanceId,
    List<string> Languages,
    string? TransferFormat = null
);
