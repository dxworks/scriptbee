using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Gateway.ProjectStructure;

public record DeleteFileCommand(ProjectId ProjectId, ScriptId Id);
