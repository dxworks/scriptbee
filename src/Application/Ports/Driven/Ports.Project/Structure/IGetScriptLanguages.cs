using OneOf;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Project.Structure;

public interface IGetScriptLanguages
{
    Task<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>> Get(
        string name,
        CancellationToken cancellationToken = default
    );
}
