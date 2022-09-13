using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;
using ScriptBee.ProjectContext;

namespace ScriptBeeWebApp.Services;

public interface IRunScriptService
{
    IEnumerable<string> GetSupportedLanguages();

    Task<RunModel?> RunAsync(Project project, ProjectModel projectModel, string language,
        string scriptFilePath, CancellationToken cancellationToken = default);
}
