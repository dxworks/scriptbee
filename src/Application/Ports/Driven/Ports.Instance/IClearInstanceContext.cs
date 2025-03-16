using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IClearInstanceContext
{
    Task Clear(InstanceInfo instanceInfo, CancellationToken cancellationToken = default);
}
