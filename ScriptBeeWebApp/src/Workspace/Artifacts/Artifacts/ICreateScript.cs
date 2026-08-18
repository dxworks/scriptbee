using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts;

public interface ICreateScript
{
    Task<ScriptId?> Create(Script script, CancellationToken cancellationToken);
}
