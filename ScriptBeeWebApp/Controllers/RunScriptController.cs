using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Services;
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
            runModel.ProjectId
        );

        if (!string.IsNullOrEmpty(runModel.Errors))
        {
            // todo add errors to database
            // runModel.ConsoleOutputName,
            // runModel.Errors
            // returnedRun.Results.Add(new OutputResult());
        }

        if (!string.IsNullOrEmpty(runModel.ConsoleOutputName))
        {
            // todo generate an unique id for each output file
            returnedRun.Results.Add(new OutputResult(runModel.ConsoleOutputName, RunResultDefaultTypes.ConsoleType,
                runModel.ConsoleOutputName));
        }

        foreach (var outputFileDatabaseName in runModel.OutputFileNames)
        {
            var (_, _, outputType, outputName) =
                _fileNameGenerator.ExtractOutputFileNameComponents(outputFileDatabaseName);

            // todo generate an unique id for each output file
            returnedRun.Results.Add(new OutputResult(outputFileDatabaseName, outputType, outputFileDatabaseName));
        }

        return Ok(returnedRun);
    }
}
