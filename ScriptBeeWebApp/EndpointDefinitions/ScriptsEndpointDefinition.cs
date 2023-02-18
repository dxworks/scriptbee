using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

public class ScriptsEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGenerateScriptService, GenerateScriptService>();
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/scripts/languages", GetLanguages);
        app.MapGet("/api/create-script", PostCreateScript);
        app.MapPost("/api/generatescript", PostGenerateScript);
    }

    public static IEnumerable<ScriptLanguage> GetLanguages(IGenerateScriptService generateScriptService)
    {
        return generateScriptService.GetSupportedLanguages();
    }

    public static async Task<IResult> PostCreateScript([FromBody] GenerateScriptRequest request,
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
