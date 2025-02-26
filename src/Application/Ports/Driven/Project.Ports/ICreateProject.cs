using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Project.Ports;

public interface ICreateProject
{
    Task<OneOf<Unit, ProjectIdAlreadyInUseError>> Create(
        ProjectDetails projectDetails,
        CancellationToken cancellationToken = default
    );
}
