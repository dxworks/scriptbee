using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Gateway.Analysis;

public record TriggerAnalysisCommand(ProjectId ProjectId, InstanceId InstanceId, ScriptId ScriptId);
