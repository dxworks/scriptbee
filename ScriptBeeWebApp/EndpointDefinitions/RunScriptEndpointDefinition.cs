using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Repository;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class RunScriptEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/scripts/run", PostRunScriptFromPath);
        // TODO: run history
    }

    [HttpPost]
    public static async Task<IResult> PostRunScriptFromPath([FromBody] RunScript runScript,
        IValidator<RunScript> validator, IRunScriptService runScriptService, IProjectManager projectManager,
        IProjectModelService projectModelService, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(runScript, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var project = projectManager.GetProject(runScript.ProjectId);
        if (project == null)
        {
            return Results.NotFound($"Could not find project with id: {runScript.ProjectId}");
        }

        var projectModel = await projectModelService.GetDocument(runScript.ProjectId, cancellationToken);
        if (projectModel == null)
        {
            return Results.NotFound($"Could not find project model with id: {runScript.ProjectId}");
        }

        // todo catch exception and remap it to a response
        var run = await runScriptService.RunAsync(project, projectModel, runScript.Language, runScript.FilePath,
            cancellationToken);

        var returnedRun = new ReturnedRun(run.Index, run.ScriptPath, run.Linker)
        {
            LoadedFiles = ConvertLoadedFiles(run.LoadedFiles),
            Results = run.Results.Select(r => new Result(r.Id, r.Type, r.Name))
                .ToList()
        };

        return Results.Ok(returnedRun);
    }

    private static Dictionary<string, List<string>> ConvertLoadedFiles(Dictionary<string, List<FileData>> loadedFiles)
    {
        return loadedFiles
            .Select(pair =>
                new KeyValuePair<string, List<string>>(pair.Key, pair.Value.Select(d => d.Name).ToList()))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}
