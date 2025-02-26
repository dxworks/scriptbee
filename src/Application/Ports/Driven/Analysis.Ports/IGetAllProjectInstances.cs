using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Analysis.Ports;

public interface IGetAllProjectInstances
{
    Task<IEnumerable<InstanceInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
