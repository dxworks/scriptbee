using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway;

public sealed record DeleteProjectCommand(ProjectId Id);
