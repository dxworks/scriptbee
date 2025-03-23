using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Project;

public interface IUpdateProject
{
    Task<ProjectDetails> Update(
        ProjectDetails projectDetails,
        CancellationToken cancellationToken = default
    );
}
