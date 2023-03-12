using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public interface IRunScriptService
{
    Task<Run> RunAsync(IProject project, ProjectModel projectModel, string language, string scriptFilePath,
        CancellationToken cancellationToken = default);
}
