using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace DxWorks.ScriptBee.Analysis.Sdk.Scripts;

public interface IScriptLoader
{
    Task<OneOf<Script, ScriptDoesNotExistsError>> Get(
        ScriptId scriptId,
        CancellationToken cancellationToken
    );

    Task<OneOf<string, ScriptDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        string path,
        CancellationToken cancellationToken
    );
}
