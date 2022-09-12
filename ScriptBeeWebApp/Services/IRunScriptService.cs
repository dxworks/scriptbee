using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;
using ScriptBee.ProjectContext;

namespace ScriptBeeWebApp.Services;

public interface IRunScriptService
{
    IScriptRunner? GetScriptRunner(string language);
    
    IEnumerable<string> GetSupportedLanguages();
    
    Task<RunModel?> RunAsync(IScriptRunner scriptRunner, Project project, ProjectModel projectModel,
        string scriptFilePath, CancellationToken cancellationToken = default);
}
