using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Gateway.ProjectStructure;
using ScriptBee.UseCases.Gateway.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class GetProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetScriptsUseCase, GetScriptsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/scripts", GetProjectScripts)
            .WithTags("Scripts")
            .WithSummary("Get all project scripts")
            .WithDescription("Retrieves a list of all scripts available in the specified project.");
        app.MapGet("/api/projects/{projectId}/scripts/{scriptId}", GetProjectScriptById)
            .WithTags("Scripts")
            .WithSummary("Get project script by ID")
            .WithDescription("Retrieves metadata about a specific project script.");
        app.MapGet("/api/projects/{projectId}/scripts/{scriptId}/content", GetProjectScriptsContent)
            .WithTags("Scripts")
            .WithSummary("Get project script content")
            .WithDescription("Retrieves the actual code content of a project script.");
    }

    private static async Task<Ok<WebGetScriptDataResponse>> GetProjectScripts(
        [FromRoute] string projectId,
        IGetScriptsUseCase useCase,
        CancellationToken cancellation
    )
    {
        var scripts = await useCase.GetAll(ProjectId.FromValue(projectId), cancellation);

        return TypedResults.Ok(
            new WebGetScriptDataResponse(scripts.Select(WebScriptData.Map).ToList())
        );
    }

    private static async Task<
        Results<Ok<WebScriptData>, NotFound<ProblemDetails>>
    > GetProjectScriptById(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string scriptId,
        IGetScriptsUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.GetById(
            ProjectId.FromValue(projectId),
            new ScriptId(scriptId),
            cancellationToken
        );

        return result.Match<Results<Ok<WebScriptData>, NotFound<ProblemDetails>>>(
            script => TypedResults.Ok(WebScriptData.Map(script)),
            error => error.ToProblem(context)
        );
    }

    private static async Task<
        Results<ContentHttpResult, NotFound<ProblemDetails>>
    > GetProjectScriptsContent(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string scriptId,
        IGetScriptsUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.GetScriptContent(
            ProjectId.FromValue(projectId),
            new ScriptId(scriptId),
            cancellationToken
        );

        return result.Match<Results<ContentHttpResult, NotFound<ProblemDetails>>>(
            content => TypedResults.Text(content),
            error => error.ToProblem(context)
        );
    }
}
