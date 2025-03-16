using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Instance;

public interface IClearInstanceContext
{
    Task Clear(InstanceInfo instanceInfo, CancellationToken cancellationToken = default);
}
