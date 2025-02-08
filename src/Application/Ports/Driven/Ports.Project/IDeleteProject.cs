using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driven.Project;

public interface IDeleteProject
{
    Task DeleteProject(ProjectId projectId, CancellationToken cancellationToken = default);
}
