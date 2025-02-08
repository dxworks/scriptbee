using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driven.Project;

public interface ICreateProject
{
    Task<OneOf<Unit, ProjectIdAlreadyInUseError>> CreateProject(ProjectDetails projectDetails,
        CancellationToken cancellationToken = default);
}
