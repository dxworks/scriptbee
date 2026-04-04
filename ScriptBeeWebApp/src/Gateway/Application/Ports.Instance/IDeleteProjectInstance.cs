using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IDeleteProjectInstance
{
    Task Delete(InstanceInfo instanceInfo, CancellationToken cancellationToken);
}
