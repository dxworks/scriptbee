using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Plugins;

public interface IGetScriptLanguages
{
    Task<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>> Get(
        InstanceInfo instanceInfo,
        string name,
        CancellationToken cancellationToken = default
    );
}
