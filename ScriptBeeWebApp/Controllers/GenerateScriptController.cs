using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class GenerateScriptController : ControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly IGenerateScriptService _generateScriptService;
    private readonly IValidator<GenerateScriptRequest> _generateScriptRequestValidator;

    public GenerateScriptController(IProjectManager projectManager, IGenerateScriptService generateScriptService,
        IValidator<GenerateScriptRequest> generateScriptRequestValidator)
    {
        _projectManager = projectManager;
        _generateScriptService = generateScriptService;
        _generateScriptRequestValidator = generateScriptRequestValidator;
    }

    [HttpGet("languages")]
    public ActionResult<IEnumerable<string>> GetLanguages()
    {
        // todo include file extension
        return Ok(_generateScriptService.GetSupportedLanguages());
    }

    [HttpPost]
    public async Task<IActionResult> PostGenerateScript([FromBody] GenerateScriptRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _generateScriptRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var scriptGeneratorStrategy = _generateScriptService.GetGenerationStrategy(request.ScriptType);

        if (scriptGeneratorStrategy is null)
        {
            return BadRequest("Invalid script type");
        }

        var project = _projectManager.GetProject(request.ProjectId);

        if (project is null)
        {
            return NotFound($"Could not find project with id: {request.ProjectId}");
        }

        var classes = project.Context.GetClasses();

        var stream =
            await _generateScriptService.GenerateClassesZip(classes, scriptGeneratorStrategy, cancellationToken);

        return File(stream, "application/octet-stream", $"{request.ScriptType}SampleCode.zip");
    }
}
