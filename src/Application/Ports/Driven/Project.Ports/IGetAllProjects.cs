using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Project.Ports;

public interface IGetAllProjects
{
    Task<IEnumerable<ProjectDetails>> GetAll(CancellationToken cancellationToken = default);
}
