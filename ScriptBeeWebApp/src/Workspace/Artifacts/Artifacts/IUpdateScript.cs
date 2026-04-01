using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts;

public interface IUpdateScript
{
    Task<Script> Update(Script script, CancellationToken cancellationToken);
}
