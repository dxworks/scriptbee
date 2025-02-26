using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Analysis;

public interface ICreateProjectInstance
{
    Task<InstanceInfo> Create(ProjectId projectId, CancellationToken cancellationToken = default);
}
