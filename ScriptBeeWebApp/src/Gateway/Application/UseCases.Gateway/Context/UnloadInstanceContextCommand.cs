using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Context;

public record UnloadInstanceContextCommand(
    ProjectId ProjectId,
    InstanceId InstanceId,
    string LoaderId,
    FileId? FileId = null
);
