using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Project;

public interface IGetAllProjects
{
    Task<IEnumerable<ProjectDetails>> GetAll(CancellationToken cancellationToken = default);
}
