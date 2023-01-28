using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;

namespace ScriptBeeWebApp.Services;

public interface ILoadersService
{
    IEnumerable<string> GetSupportedLoaders();

    IModelLoader? GetLoader(string name);

    ISet<string> GetAcceptedModules();

    Task<Dictionary<string, List<string>>> LoadFiles(ProjectModel projectModel, List<Node> loadModelsNodes,
        CancellationToken cancellationToken = default);

    Task<Dictionary<string, List<string>>> ReloadModels(ProjectModel projectModel,
        CancellationToken cancellationToken = default);
}
