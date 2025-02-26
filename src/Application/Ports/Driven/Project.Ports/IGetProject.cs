using OneOf;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Project.Ports;

public interface IGetProject
{
    Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> GetById(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
