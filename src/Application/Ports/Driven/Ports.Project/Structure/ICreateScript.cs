using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Project.Structure;

public interface ICreateScript
{
    Task Create(Script script, CancellationToken cancellationToken = default);
}
