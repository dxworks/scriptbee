using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Project;

public interface ICreateProject
{
    Task<OneOf<Success, ProjectIdAlreadyInUseError>> Create(
        ProjectDetails projectDetails,
        CancellationToken cancellationToken
    );
}
