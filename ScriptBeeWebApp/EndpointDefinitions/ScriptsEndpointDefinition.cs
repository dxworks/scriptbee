using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Repository;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

public class ScriptsEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO: reduce the number of services that are required in the controller and only register the ones that are needed
        services.AddSingleton<IScriptsService, ScriptsService>();
        services.AddSingleton<IGenerateScriptService, GenerateScriptService>();
        services.AddSingleton<IScriptModelService, ScriptModelService>();
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/scripts", GetScripts);
        app.MapPost("/api/scripts", PostCreateScript);
        app.MapPut("/api/scripts", PutUpdateScript);
        app.MapGet("/api/scripts/{scriptId}", GetScriptById);
        app.MapDelete("/api/scripts/{scriptId}", DeleteScript);
        app.MapGet("/api/scripts/{scriptId}/content", GetScriptContent);
        app.MapGet("/api/scripts/languages", GetLanguages);
        app.MapPost("/api/generatescript", PostGenerateScript);
    }

    private static IEnumerable<ScriptLanguage> GetLanguages(IScriptsService scriptsService)
    {
        return scriptsService.GetSupportedLanguages();
    }

    private async Task<IResult> GetScripts(IScriptsService scriptsService, [FromQuery] string projectId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return Results.BadRequest(new ValidationErrorsResponse(new List<ValidationError>
            {
                new("projectId", "ProjectId is required")
            }));
        }

        var result = await scriptsService.GetScriptsStructureAsync(projectId, cancellationToken);

        return result.Match(
            Results.Ok,
            Results.NotFound);
    }

    private static async Task<IResult> GetScriptById(IScriptsService scriptsService, [FromRoute] string scriptId,
        [FromQuery] string projectId, CancellationToken cancellationToken = default)
    {
        var result = await scriptsService.GetScriptByFilePathAsync(scriptId, projectId, cancellationToken);

        return result.Match(
            Results.Ok,
            Results.NotFound,
            Results.NotFound);
    }

    private async Task<IResult> GetScriptContent(IScriptsService scriptsService, [FromRoute] string scriptId,
        [FromQuery] string projectId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return Results.BadRequest(new ValidationErrorsResponse(new List<ValidationError>
            {
                new("projectId", "ProjectId is required")
            }));
        }

        var result = await scriptsService.GetScriptContentAsync(scriptId, projectId, cancellationToken);

        return result.Match(
            Results.Ok,
            Results.NotFound,
            Results.NotFound);
    }

    private static async Task<IResult> PostCreateScript([FromBody] CreateScript createScript,
        IValidator<CreateScript> validator, IScriptsService scriptsService,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(createScript, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var result = await scriptsService.CreateScriptAsync(createScript, cancellationToken);

        return result.Match(
            Results.Ok,
            Results.NotFound,
            Results.Conflict,
            Results.BadRequest);
    }

    private static async Task<IResult> PutUpdateScript([FromBody] UpdateScript updateScript,
        IValidator<UpdateScript> validator, IScriptsService scriptsService,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(updateScript, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var result = await scriptsService.UpdateScriptAsync(updateScript, cancellationToken);

        return result.Match(
            Results.Ok,
            Results.NotFound,
            Results.NotFound);
    }

    private static async Task<IResult> DeleteScript(IScriptsService scriptsService, [FromRoute] string scriptId,
        [FromQuery] string projectId, CancellationToken cancellationToken = default)
    {
        var result = await scriptsService.DeleteScriptAsync(scriptId, projectId, cancellationToken);

        return result.Match(
            Results.Ok,
            Results.NotFound,
            Results.NotFound);
    }

// todo pact add tests

    public static async Task<IResult> PostGenerateScript([FromBody] GenerateScriptRequest request,
        IValidator<GenerateScriptRequest> validator, IGenerateScriptService generateScriptService,
        IProjectManager projectManager, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var scriptGeneratorStrategy = generateScriptService.GetGenerationStrategy(request.ScriptType);

        if (scriptGeneratorStrategy is null)
        {
            return Results.BadRequest("Invalid script type");
        }

        var project = projectManager.GetProject(request.ProjectId);

        if (project is null)
        {
            return Results.NotFound($"Could not find project with id: {request.ProjectId}");
        }

        var classes = project.Context.GetClasses();

        var stream =
            await generateScriptService.GenerateClassesZip(classes, scriptGeneratorStrategy, cancellationToken);

        return Results.File(stream, "application/octet-stream", $"{request.ScriptType}SampleCode.zip");
    }
}
