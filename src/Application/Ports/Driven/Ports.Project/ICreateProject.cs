using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driven.Projects;

public interface ICreateProject
{
    Task<OneOf<Unit, ProjectIdAlreadyInUseError>> CreateProject(ProjectDetails projectDetails,
        CancellationToken cancellationToken = default);
}
