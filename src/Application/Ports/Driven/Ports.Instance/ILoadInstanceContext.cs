using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance;

public interface ILoadInstanceContext
{
    Task Load(
        InstanceInfo instanceInfo,
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken = default
    );
}
