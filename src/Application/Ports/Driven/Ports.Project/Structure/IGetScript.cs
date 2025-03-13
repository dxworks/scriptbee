using OneOf;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Project.Structure;

public interface IGetScript
{
    Task<OneOf<Script, ScriptDoesNotExistsError>> Get(
        ScriptId scriptId,
        CancellationToken cancellationToken = default
    );
}
