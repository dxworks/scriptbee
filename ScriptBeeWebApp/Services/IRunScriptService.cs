using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.ScriptRunner;
using ScriptBee.Models;
using ScriptBee.ProjectContext;

namespace ScriptBeeWebApp.Services;

public interface IRunScriptService
{
    Task<RunModel?> RunAsync(IScriptRunner scriptRunner, Project project, ProjectModel projectModel,
        string scriptFilePath, CancellationToken cancellationToken = default);
}
