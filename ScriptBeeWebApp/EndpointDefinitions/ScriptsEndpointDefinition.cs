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
        app.MapGet("/api/scripts/languages", GetLanguages);
        app.MapPost("/api/scripts", PostCreateScript);
        app.MapPost("/api/generatescript", PostGenerateScript);
    }

    public static IEnumerable<ScriptLanguage> GetLanguages(IScriptsService scriptsService)
    {
        return scriptsService.GetSupportedLanguages();
    }

    public static async Task<IResult> PostCreateScript([FromBody] CreateScript createScript,
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
