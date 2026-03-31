using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class GetProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
        services.AddSingleton<IGetScriptsUseCase, GetScriptsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/scripts", GetProjectScripts).WithTags("Scripts");
        app.MapGet("/api/projects/{projectId}/scripts/{scriptId}", GetProjectScriptById)
            .WithTags("Scripts");
        app.MapGet("/api/projects/{projectId}/scripts/{scriptId}/content", GetProjectScriptsContent)
            .WithTags("Scripts");
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

    private static async Task<Ok<string>> GetProjectScriptsContent(
        [FromRoute] string projectId,
        [FromRoute] string scriptId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value
        return TypedResults.Ok(scriptId);
    }
}
