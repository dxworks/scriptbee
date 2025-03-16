using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Context;

public record ClearContextCommand(ProjectId ProjectId, InstanceId InstanceId);
