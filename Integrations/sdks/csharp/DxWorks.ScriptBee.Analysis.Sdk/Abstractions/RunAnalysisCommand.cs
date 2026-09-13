using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public record RunAnalysisCommand(ProjectId ProjectId, ScriptId ScriptId);
