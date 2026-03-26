using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts;

public interface ICreateScript
{
    Task Create(Script script, CancellationToken cancellationToken = default);
}

