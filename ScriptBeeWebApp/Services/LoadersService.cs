using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers.Arguments;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class LoadersService : ILoadersService
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
        return _pluginRepository.GetLoadedPluginManifests()
            .Where(manifest => manifest.ExtensionPoints.Any(extensionPoint => extensionPoint.Kind == PluginKind.Loader))
            .Select(manifest => manifest.Name);
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

    public async Task<Dictionary<string, List<FileData>>> LoadFiles(ProjectModel projectModel,
        List<Node> loadModelsNodes, CancellationToken cancellationToken = default)
    {
        var loadedFiles = await SaveLoadedFilesInProjectModelAsync(projectModel, loadModelsNodes, cancellationToken);

        await LoadModelFiles(projectModel.Id, loadedFiles, cancellationToken);

        return loadedFiles;
    }

    public async Task<Dictionary<string, List<FileData>>> ReloadModels(ProjectModel projectModel,
        CancellationToken cancellationToken = default)
    {
        await LoadModelFiles(projectModel.Id, projectModel.LoadedFiles, cancellationToken);

        return projectModel.LoadedFiles;
    }

    private async Task LoadModelFiles(string projectId, Dictionary<string, List<FileData>> loadedFiles,
        CancellationToken cancellationToken)
    {
        foreach (var (loader, loadedFileData) in loadedFiles)
        {
            var modelLoader = GetLoader(loader);

            if (modelLoader is null)
            {
                continue;
            }

            List<Stream> loadedFileStreams = new();

            foreach (var fileData in loadedFileData)
            {
                var fileStream = await _fileModelService.GetFileAsync(fileData.Id.ToString());
                loadedFileStreams.Add(fileStream);
            }

            var dictionary = await modelLoader.LoadModel(loadedFileStreams, cancellationToken: cancellationToken);

            _projectManager.AddToGivenProject(projectId, dictionary, modelLoader.GetName());

            foreach (var fileStream in loadedFileStreams)
            {
                await fileStream.DisposeAsync();
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
