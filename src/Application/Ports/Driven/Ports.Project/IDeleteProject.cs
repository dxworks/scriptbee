using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Project;

public interface IDeleteProject
{
    Task Delete(ProjectId projectId, CancellationToken cancellationToken = default);
}
