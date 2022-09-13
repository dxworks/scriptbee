using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class RunScriptController : ControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IProjectModelService _projectModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IRunScriptService _runScriptService;
    private readonly IValidator<RunScript> _runScriptValidator;

    public RunScriptController(IProjectManager projectManager, IProjectModelService projectModelService,
        IFileNameGenerator fileNameGenerator, IRunScriptService runScriptService,
        IValidator<RunScript> runScriptValidator)
    {
        _projectManager = projectManager;
        _projectModelService = projectModelService;
        _fileNameGenerator = fileNameGenerator;
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

        var runModel = await _runScriptService.RunAsync(project, projectModel, runScript.Language, runScript.FilePath,
            cancellationToken);

        if (runModel is null)
        {
            return NotFound($"File from {runScript.FilePath} not found");
        }

        ReturnedRun returnedRun = new
        (
            runModel.Id,
            runModel.RunIndex,
            runModel.ProjectId,
            runModel.ConsoleOutputName,
            runModel.Errors
        );

        var outputFiles = runModel.OutputFileNames.Select(outputFileDatabaseName =>
        {
            var (_, _, outputType, outputName) =
                _fileNameGenerator.ExtractOutputFileNameComponents(outputFileDatabaseName);

            return new OutputFile(outputName, outputType, outputFileDatabaseName);
        }).ToList();


        returnedRun.OutputFiles = outputFiles;

        return Ok(returnedRun);
    }
}
