using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driving.UseCases.Project;

public record DeleteProjectCommand(ProjectId Id);
