using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Project.Ports;

public interface IDeleteProject
{
    Task Delete(ProjectId projectId, CancellationToken cancellationToken = default);
}
