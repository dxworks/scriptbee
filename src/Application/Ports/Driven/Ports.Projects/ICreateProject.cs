using ScriptBee.Domain.Model.Projects;

namespace ScriptBee.Ports.Driven.Projects;

public interface ICreateProject
{
    Task CreateProject(Project project, CancellationToken cancellationToken = default);
}
