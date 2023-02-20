using OneOf;
using ScriptBeeWebApp.Data;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.DTO;

namespace ScriptBeeWebApp.Services;

public interface IScriptsService
{
    IEnumerable<ScriptLanguage> GetSupportedLanguages();

    Task<OneOf<CreateScriptResponse, ProjectMissing, ScriptConflict, InvalidScriptType>> CreateScriptAsync(
        CreateScript createScript, CancellationToken cancellationToken = default);
}
