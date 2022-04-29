using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Config;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class UploadModelController : ControllerBase
{
    private readonly ILoadersHolder _loadersHolder;
    private readonly IProjectManager _projectManager;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IFileModelService _fileModelService;
    private readonly IProjectModelService _projectModelService;

    public UploadModelController(ILoadersHolder loadersHolder, IProjectManager projectManager,
        IFileNameGenerator fileNameGenerator, IFileModelService fileModelService,
        IProjectModelService projectModelService)
    {
        _loadersHolder = loadersHolder;
        _projectManager = projectManager;
        _fileNameGenerator = fileNameGenerator;
        _fileModelService = fileModelService;
        _projectModelService = projectModelService;
    }

    [HttpPost("fromfile")]
    public async Task<IActionResult> UploadFromFile(IFormCollection formData, CancellationToken cancellationToken)
    {
        if (!formData.TryGetValue("loaderName", out var loaderName))
        {
            return BadRequest("Missing loader name");
        }

        if (!formData.TryGetValue("projectId", out var projectId))
        {
            return BadRequest("Missing project id");
        }

        var savedFiles = new List<string>();

        foreach (var file in formData.Files)
        {
            if (file.Length > 0)
            {
                var modelName = _fileNameGenerator.GenerateModelName(projectId, file.FileName, loaderName[0]);

                savedFiles.Add(modelName);

                await using var stream = file.OpenReadStream();
                await _fileModelService.UploadFile(modelName, stream, cancellationToken);
            }
        }

        var projectModel = await _projectModelService.GetDocument(projectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {projectId}");
        }

        projectModel.SavedFiles[loaderName[0]] = savedFiles;

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Ok();
    }

    [HttpPost("frompath")]
    public async Task<IActionResult> UploadFromPath(ScriptLoaderArguments scriptLoaderArguments)
    {
        if (scriptLoaderArguments.loaderName == null)
        {
            return BadRequest("Missing loader name");
        }

        if (scriptLoaderArguments.projectId == null)
        {
            return BadRequest("Missing project id");
        }

        var project = _projectManager.GetProject(scriptLoaderArguments.projectId);

        if (project == null)
        {
            return NotFound($"Could not find project with id: {scriptLoaderArguments.projectId}");
        }

        var modelLoader = _loadersHolder.GetModelLoader(scriptLoaderArguments.loaderName);

        if (modelLoader == null)
        {
            return BadRequest($"Model type {scriptLoaderArguments.loaderName} is not supported");
        }

        var fileStreams = new List<Stream>();

        var wrongPaths = new List<string>();

        foreach (var modelPath in scriptLoaderArguments.paths)
        {
            var filePath = Path.Combine(ConfigFolders.PathToModels, modelPath);
            try
            {
                fileStreams.Add(System.IO.File.OpenRead(filePath));
            }
            catch (Exception)
            {
                wrongPaths.Add(modelPath);
            }
        }

        if (wrongPaths.Count != 0)
        {
            string badRequestMessage = "Could not load models from the following paths:\n";
            foreach (var wrongPath in wrongPaths)
            {
                badRequestMessage = badRequestMessage + wrongPath + "\n";
            }

            return BadRequest(badRequestMessage);
        }

        var dictionary = await modelLoader.LoadModel(fileStreams);

        _projectManager.AddToGivenProject(scriptLoaderArguments.projectId, dictionary, modelLoader.GetName());

        foreach (var fileStream in fileStreams)
        {
            await fileStream.DisposeAsync();
        }

        return Ok();
    }
}