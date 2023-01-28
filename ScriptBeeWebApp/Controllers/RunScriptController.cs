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
using ScriptBeeWebApp.Controllers.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class RunScriptController : ControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IProjectModelService _projectModelService;
    private readonly IRunScriptService _runScriptService;
    private readonly IValidator<RunScript> _runScriptValidator;

    public RunScriptController(IProjectManager projectManager, IProjectModelService projectModelService,
        IRunScriptService runScriptService, IValidator<RunScript> runScriptValidator)
    {
        _projectManager = projectManager;
        _projectModelService = projectModelService;
        _runScriptService = runScriptService;
        _runScriptValidator = runScriptValidator;
    }

    [HttpGet("languages")]
    public ActionResult<IEnumerable<string>> GetLanguages()
    {
        return Ok(_runScriptService.GetSupportedLanguages());
    }

    [HttpPost]
    public async Task<IActionResult> RunScriptFromPath(RunScript runScript,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _runScriptValidator.ValidateAsync(runScript, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var project = _projectManager.GetProject(runScript.ProjectId);
        if (project == null)
        {
            return NotFound($"Could not find project with id: {runScript.ProjectId}");
        }

        var projectModel = await _projectModelService.GetDocument(runScript.ProjectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {runScript.ProjectId}");
        }

        // todo catch exception and remap it to a response
        var run = await _runScriptService.RunAsync(project, projectModel, runScript.Language, runScript.FilePath,
            cancellationToken);

        var returnedRun = new ReturnedRun(run.Index, run.ScriptPath, run.Linker)
        {
            LoadedFiles = ConvertLoadedFiles(run.LoadedFiles),
            Results = run.Results.Select(r => new Result(r.Id, r.Type, r.Name))
                .ToList()
        };

        return Ok(returnedRun);
    }

    private static Dictionary<string, List<string>> ConvertLoadedFiles(Dictionary<string, List<FileData>> loadedFiles)
    {
        return loadedFiles
            .Select(pair =>
                new KeyValuePair<string, List<string>>(pair.Key, pair.Value.Select(d => d.Name).ToList()))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}
