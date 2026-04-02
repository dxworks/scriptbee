using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts;

public interface IDeleteScript
{
    Task<ProjectStructureEntry?> Delete(ScriptId id, CancellationToken cancellationToken);
}
