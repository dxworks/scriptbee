using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Models;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.Repository;

namespace ScriptBeeWebApp.Services;

// todo add tests
public sealed class LoadersService : ILoadersService
{
    private readonly IFileModelService _fileModelService;
    private readonly IPluginRepository _pluginRepository;
    private readonly IProjectManager _projectManager;
    private readonly IProjectModelService _projectModelService;

    public LoadersService(IPluginRepository pluginRepository, IProjectModelService projectModelService,
        IProjectManager projectManager, IFileModelService fileModelService)
    {
        _pluginRepository = pluginRepository;
        _projectModelService = projectModelService;
        _projectManager = projectManager;
        _fileModelService = fileModelService;
    }

    public IEnumerable<string> GetSupportedLoaders()
    {
        return _pluginRepository.GetLoadedPlugins(PluginKind.Loader)
            .Select(plugin => plugin.Id);
    }

    public IModelLoader? GetLoader(string name)
    {
        return _pluginRepository.GetPlugin<IModelLoader>(strategy => strategy.GetName() == name);
    }

    public ISet<string> GetAcceptedModules()
    {
        var acceptedModules = new HashSet<string>();

        foreach (var modelLoader in _pluginRepository.GetPlugins<IModelLoader>())
        {
            acceptedModules.Add(modelLoader.GetType().Module.Name);
        }

        return acceptedModules;
    }

    public async Task<Dictionary<string, List<string>>> LoadFiles(ProjectModel projectModel,
        List<Node> loadModelsNodes, CancellationToken cancellationToken = default)
    {
        var loadedFiles = await SaveLoadedFilesInProjectModelAsync(projectModel, loadModelsNodes, cancellationToken);

        var loadModels = await LoadModelFiles(projectModel.Id, loadedFiles, cancellationToken);

        return loadModels;
    }

    public async Task<Dictionary<string, List<string>>> ReloadModels(ProjectModel projectModel,
        CancellationToken cancellationToken = default)
    {
        var loadedModels = await LoadModelFiles(projectModel.Id, projectModel.LoadedFiles, cancellationToken);

        return loadedModels;
    }

    private async Task<Dictionary<string, List<string>>> LoadModelFiles(string projectId,
        Dictionary<string, List<FileData>> loadedFiles, CancellationToken cancellationToken)
    {
        var loadModels = new Dictionary<string, List<string>>();

        foreach (var (loader, loadedFileData) in loadedFiles)
        {
            var modelLoader = GetLoader(loader);

            if (modelLoader is null)
            {
                continue;
            }

            List<NamedFileStream> loadedFileStreams = new();

            foreach (var fileName in loadedFileData.Select(fileData => fileData.Id.ToString()))
            {
                var fileStream = await _fileModelService.GetFileAsync(fileName);
                loadedFileStreams.Add(new NamedFileStream(fileName, fileStream));
            }

            var dictionary = await modelLoader.LoadModel(loadedFileStreams, cancellationToken: cancellationToken);

            _projectManager.AddToGivenProject(projectId, dictionary, modelLoader.GetName());

            AddToLoadedModels(dictionary, loader, loadModels);

            foreach (var fileStream in loadedFileStreams)
            {
                await fileStream.Stream.DisposeAsync();
            }
        }

        return loadModels;
    }

    private static void AddToLoadedModels(Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary,
        string loader, IDictionary<string, List<string>> loadModels)
    {
        foreach (var model in dictionary.Keys)
        {
            if (loadModels.TryGetValue(model, out var loaders))
            {
                loaders.Add(loader);
            }
            else
            {
                loadModels.Add(model, new List<string> { loader });
            }
        }
    }

    private async Task<Dictionary<string, List<FileData>>> SaveLoadedFilesInProjectModelAsync(ProjectModel projectModel,
        List<Node> loadModelsNodes, CancellationToken cancellationToken)
    {
        Dictionary<string, List<FileData>> loadedFiles = new();

        foreach (var (loaderName, modelFileNames) in loadModelsNodes)
        {
            if (projectModel.SavedFiles.TryGetValue(loaderName, out var savedFiles))
            {
                var files = savedFiles.Where(file => modelFileNames.Contains(file.Name))
                    .ToList();

                loadedFiles[loaderName] = files;
            }
        }

        projectModel.LoadedFiles = loadedFiles;

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);
        return loadedFiles;
    }
}
