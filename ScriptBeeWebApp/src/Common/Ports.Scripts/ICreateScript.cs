using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Scripts;

public interface ICreateScript
{
    Task Create(Script script, CancellationToken cancellationToken = default);
}
