using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface IGenerateInstanceClasses
{
    Task<Stream> Generate(
        InstanceInfo instanceInfo,
        List<string> languages,
        string? transferFormat,
        CancellationToken cancellationToken
    );
}
