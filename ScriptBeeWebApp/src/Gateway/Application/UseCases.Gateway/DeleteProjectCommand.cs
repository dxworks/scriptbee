using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project;

public sealed record DeleteProjectCommand(ProjectId Id);
