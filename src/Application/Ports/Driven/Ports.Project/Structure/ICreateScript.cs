using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Project.Structure;

public interface ICreateScript
{
    public Task Create(Script script, CancellationToken cancellationToken = default);
}
