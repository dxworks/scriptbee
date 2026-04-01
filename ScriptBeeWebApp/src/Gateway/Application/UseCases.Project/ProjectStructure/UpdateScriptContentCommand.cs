using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public record UpdateScriptContentCommand(ProjectId ProjectId, ScriptId ScriptId, string Content);
