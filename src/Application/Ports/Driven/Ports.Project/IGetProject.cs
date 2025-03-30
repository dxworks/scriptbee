using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Project;

public interface IGetProject
{
    Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> GetById(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
