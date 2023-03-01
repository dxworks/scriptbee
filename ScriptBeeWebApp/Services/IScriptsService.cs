using OneOf;
using ScriptBeeWebApp.Data;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.DTO;

namespace ScriptBeeWebApp.Services;

public interface IScriptsService
{
    IEnumerable<ScriptLanguage> GetSupportedLanguages();

    Task<OneOf<IEnumerable<ScriptFileStructureNode>, ProjectMissing>> GetScriptsStructureAsync(string projectId,
        CancellationToken cancellationToken = default);

    Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> GetScriptByFilePathAsync(string filepath,
        string projectId, CancellationToken cancellationToken = default);

    Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> GetScriptByIdAsync(string scriptId, string projectId,
        CancellationToken cancellationToken = default);

    Task<OneOf<string, ProjectMissing, ScriptMissing>> GetScriptContentAsync(string scriptId, string projectId,
        CancellationToken cancellationToken = default);

    Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptConflict, InvalidScriptType>> CreateScriptAsync(
        CreateScript createScript, CancellationToken cancellationToken = default);

    Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> UpdateScriptAsync(UpdateScript updateScript,
        CancellationToken cancellationToken = default);

    Task<OneOf<ScriptDataResponse, ProjectMissing, ScriptMissing>> DeleteScriptAsync(string scriptId, string projectId,
        CancellationToken cancellationToken);
}
