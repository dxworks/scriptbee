using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Context;

public record ClearContextCommand(ProjectId ProjectId, InstanceId InstanceId);
