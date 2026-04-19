using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Context;

public sealed record GenerateClassesCommand(
    ProjectId ProjectId,
    InstanceId InstanceId,
    List<string> Languages,
    string? TransferFormat = null
);
