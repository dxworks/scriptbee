using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IGetProjectInstance
{
    Task<OneOf<InstanceInfo, InstanceDoesNotExistsError>> Get(
        InstanceId id,
        CancellationToken cancellationToken = default
    );
}
