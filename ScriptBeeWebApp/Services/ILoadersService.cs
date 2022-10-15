using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;
using ScriptBeeWebApp.Controllers.Arguments;

namespace ScriptBeeWebApp.Services;

public interface ILoadersService
{
    IEnumerable<string> GetSupportedLoaders();

    IModelLoader? GetLoader(string name);

    ISet<string> GetAcceptedModules();

    Task<Dictionary<string, List<FileData>>> LoadFiles(ProjectModel projectModel, List<Node> loadModelsNodes,
        CancellationToken cancellationToken = default);

    Task<Dictionary<string, List<FileData>>> ReloadModels(ProjectModel projectModel,
        CancellationToken cancellationToken = default);
}
