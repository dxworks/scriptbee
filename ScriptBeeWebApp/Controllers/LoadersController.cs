using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
// todo add tests
public class LoadersController : ControllerBase
{
    private readonly ILoadersService _loadersService;
    private readonly IValidator<LoadModels> _loadModelsValidator;
    private readonly IProjectManager _projectManager;
    private readonly IProjectModelService _projectModelService;
    private readonly IProjectStructureService _projectStructureService;

    public LoadersController(ILoadersService loadersService, IProjectModelService projectModelService,
        IProjectManager projectManager, IProjectStructureService projectStructureService,
        IValidator<LoadModels> loadModelsValidator)
    {
        _loadersService = loadersService;
        _projectModelService = projectModelService;
        _projectManager = projectManager;
        _projectStructureService = projectStructureService;
        _loadModelsValidator = loadModelsValidator;
    }

    [HttpGet]
    public ActionResult<IEnumerable<string>> GetAllProjectLoaders()
    {
        return Ok(_loadersService.GetSupportedLoaders());
    }

    [HttpPost]
    public async Task<IActionResult> LoadFiles(LoadModels loadModels, CancellationToken cancellationToken = default)
    {
        var validationResult = await _loadModelsValidator.ValidateAsync(loadModels, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.GetValidationErrorsResponse());
        }

        // todo maybe move into a validator and use di for it to get LoaderService
        foreach (var (loader, _) in loadModels.Nodes)
        {
            var modelLoader = _loadersService.GetLoader(loader);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {loader} is not supported");
            }
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

        var loadFiles = await _loadersService.LoadFiles(projectModel, loadModels.Nodes, cancellationToken);

        await _projectStructureService.GenerateModelClasses(loadModels.ProjectId, cancellationToken);

        return Ok(ConvertLoadedFiles(loadFiles));
    }

    [HttpPost("{projectId}")]
    // todo extract validation to separate class
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

        // todo maybe move into a validator and use di for it to get LoaderService
        foreach (var (loader, _) in projectModel.LoadedFiles)
        {
            var modelLoader = _loadersService.GetLoader(loader);

            if (modelLoader == null)
            {
                return BadRequest($"Model type {loader} is not supported");
            }
        }


        var project = _projectManager.GetProject(projectId);
        if (project == null)
        {
            _projectManager.LoadProject(projectModel);
        }

        var loadFiles = await _loadersService.ReloadModels(projectModel, cancellationToken);

        return Ok(ConvertLoadedFiles(loadFiles));
    }

    [HttpPost("clear/{projectId}")]
    // todo extract validation to separate class
    public async Task<IActionResult> ClearProjectContext(string projectId, CancellationToken cancellationToken)
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

        var project = _projectManager.GetProject(projectId);
        if (project != null)
        {
            project.Context.Clear();
        }

        projectModel.LoadedFiles.Clear();

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Ok();
    }

    private static IEnumerable<ReturnedNode> ConvertLoadedFiles(Dictionary<string, List<FileData>> loadFiles)
    {
        return loadFiles.Select(pair => new ReturnedNode(pair.Key, pair.Value.Select(d => d.Name).ToList()));
    }
}
