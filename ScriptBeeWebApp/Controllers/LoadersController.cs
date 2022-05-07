using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class LoadersController : ControllerBase
{
    private readonly ILoadersHolder _loadersHolder;
    private readonly IProjectModelService _projectModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IFileModelService _fileModelService;
    private readonly IProjectManager _projectManager;

    public LoadersController(ILoadersHolder loadersHolder, IProjectModelService projectModelService,
        IFileNameGenerator fileNameGenerator, IFileModelService fileModelService, IProjectManager projectManager)
    {
        _loadersHolder = loadersHolder;
        _projectModelService = projectModelService;
        _fileNameGenerator = fileNameGenerator;
        _fileModelService = fileModelService;
        _projectManager = projectManager;
    }

    [HttpGet]
    public IActionResult GetAllProjectLoaders()
    {
        return Ok(_loadersHolder.GetAllLoaders().Select(modelLoader => modelLoader.GetName()).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> LoadFiles(LoadModels loadModels, CancellationToken cancellationToken)
    {
        if (loadModels == null || string.IsNullOrWhiteSpace(loadModels.ProjectId))
        {
            return BadRequest("Invalid arguments. ProjectId needed!");
        }

        var projectModel = await _projectModelService.GetDocument(loadModels.ProjectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {loadModels.ProjectId}");
        }

        var project = _projectManager.GetProject(loadModels.ProjectId);

        if (project == null)
        {
            _projectManager.LoadProject(projectModel);
        }

        Dictionary<string, List<string>> loadedFiles = new();

        foreach (var (loaderName, models) in loadModels.Nodes)
        {
            List<string> modelNames = new();

            foreach (var model in models)
            {
                var modelName = _fileNameGenerator.GenerateModelName(loadModels.ProjectId, loaderName, model);
                modelNames.Add(modelName);
            }

            loadedFiles[loaderName] = modelNames;
        }

        projectModel.LoadedFiles = loadedFiles;
        projectModel.Loaders = _loadersHolder.GetAllLoaders().Select(modelLoader => modelLoader.GetName()).ToList();

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        foreach (var (loader, loadedFileNames) in loadedFiles)
        {
            List<Stream> loadedFileStreams = new();

            var modelLoader = _loadersHolder.GetModelLoader(loader);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {loader} is not supported");
            }

            foreach (var loadedFileName in loadedFileNames)
            {
                var fileStream = await _fileModelService.GetFile(loadedFileName);
                loadedFileStreams.Add(fileStream);
            }

            var dictionary = await modelLoader.LoadModel(loadedFileStreams, cancellationToken: cancellationToken);

            _projectManager.AddToGivenProject(loadModels.ProjectId, dictionary, modelLoader.GetName());

            foreach (var fileStream in loadedFileStreams)
            {
                await fileStream.DisposeAsync();
            }
        }

        return Ok();
    }

    [HttpPost("{projectId}")]
    public async Task<IActionResult> ReloadProjectContext(string projectId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            return BadRequest("Invalid argument. ProjectId needed!");
        }

        var projectModel = await _projectModelService.GetDocument(projectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {projectId}");
        }

        if (projectModel.LoadedFiles.Count == 0)
        {
            return Ok();
        }

        var project = _projectManager.GetProject(projectId);
        if (project == null)
        {
            _projectManager.LoadProject(projectModel);
        }

        foreach (var (loader, loadedFileNames) in projectModel.LoadedFiles)
        {
            List<Stream> loadedFileStreams = new();

            var modelLoader = _loadersHolder.GetModelLoader(loader);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {loader} is not supported");
            }

            foreach (var loadedFileName in loadedFileNames)
            {
                var fileStream = await _fileModelService.GetFile(loadedFileName);
                loadedFileStreams.Add(fileStream);
            }

            var dictionary = await modelLoader.LoadModel(loadedFileStreams, cancellationToken: cancellationToken);

            _projectManager.AddToGivenProject(projectId, dictionary, modelLoader.GetName());

            foreach (var fileStream in loadedFileStreams)
            {
                await fileStream.DisposeAsync();
            }
        }

        return Ok();
    }
}