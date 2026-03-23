using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Analysis;

public record RunAnalysisCommand(ProjectId ProjectId, ScriptId ScriptId);
