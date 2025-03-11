using OneOf;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project.Structure;

namespace ScriptBee.Persistence.File;

public class GetScriptLanguagesAdapter : IGetScriptLanguages
{
    public async Task<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>> Get(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        // TODO FIXIT(#34): read from loaded plugins of type script runner
        await Task.CompletedTask;

        return new ScriptLanguage(name, name);
    }
}
