using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using Microsoft.Extensions.Logging;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.File;
using ScriptBee.Plugins.Loader;

namespace ScriptBee.Service.Analysis;

public class LoadModelFilesService(
    IProjectManager projectManager,
    IPluginRepository pluginRepository,
    IFileModelService fileModelService,
    ILogger<LoadModelFilesService> logger
) : ILoadModelFilesService
{
    public async Task LoadModelFiles(
        IDictionary<string, IEnumerable<FileId>> loadedFiles,
        CancellationToken cancellationToken = default
    )
    {
        var loadModels = new Dictionary<string, List<string>>();

        foreach (var (loader, fileIds) in loadedFiles)
        {
            var modelLoader = GetLoader(loader);

            if (modelLoader is null)
            {
                logger.LogWarning(
                    "No model loader plugin found for loader '{LoaderId}' — files will be skipped",
                    loader
                );
                continue;
            }

            logger.LogDebug("Loading model files with loader '{LoaderId}'", loader);
            await LoadModelFiles(fileIds, modelLoader, loader, loadModels, cancellationToken);
        }
    }

    private async Task LoadModelFiles(
        IEnumerable<FileId> fileIds,
        IModelLoader modelLoader,
        string loader,
        Dictionary<string, List<string>> loadModels,
        CancellationToken cancellationToken
    )
    {
        List<NamedFileStream> loadedFileStreams = [];

        foreach (var fileId in fileIds)
        {
            var fileStream = await fileModelService.GetFileAsync(fileId, cancellationToken);
            loadedFileStreams.Add(new NamedFileStream(fileId.ToString(), fileStream));
        }

        var dictionary = await modelLoader.LoadModel(
            loadedFileStreams,
            cancellationToken: cancellationToken
        );

        logger.LogDebug(
            "Loader '{LoaderId}' produced {TypeCount} model type(s)",
            loader,
            dictionary.Count
        );

        projectManager.AddToGivenProject(dictionary, modelLoader.GetName());

        AddToLoadedModels(dictionary, loader, loadModels);

        foreach (var fileStream in loadedFileStreams)
        {
            await fileStream.Stream.DisposeAsync();
        }
    }

    private static void AddToLoadedModels(
        Dictionary<string, Dictionary<string, ScriptBeeModel>> dictionary,
        string loader,
        Dictionary<string, List<string>> loadModels
    )
    {
        foreach (var model in dictionary.Keys)
        {
            if (loadModels.TryGetValue(model, out var loaders))
            {
                loaders.Add(loader);
            }
            else
            {
                loadModels.Add(model, [loader]);
            }
        }
    }

    private IModelLoader? GetLoader(string name)
    {
        return pluginRepository.GetPlugin<IModelLoader>(strategy => strategy.GetName() == name);
    }
}
