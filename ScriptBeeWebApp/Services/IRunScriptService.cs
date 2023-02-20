using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public interface IRunScriptService
{
    IEnumerable<string> GetSupportedLanguages();

    Task<Run> RunAsync(IProject project, ProjectModel projectModel, string language, string scriptFilePath,
        CancellationToken cancellationToken = default);
}
