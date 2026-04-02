using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public record DeleteFileCommand(ProjectId ProjectId, ScriptId Id);
