using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driven.Project;

public interface IGetAllProjects
{
    Task<IEnumerable<ProjectDetails>> GetAll(CancellationToken cancellationToken = default);
}
