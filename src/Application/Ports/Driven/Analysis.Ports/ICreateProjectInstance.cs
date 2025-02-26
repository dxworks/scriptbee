using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Analysis.Ports;

public interface ICreateProjectInstance
{
    Task<InstanceInfo> Create(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
