using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IGenerateInstanceClasses
{
    Task Generate(InstanceInfo instanceInfo, CancellationToken cancellationToken);
}
