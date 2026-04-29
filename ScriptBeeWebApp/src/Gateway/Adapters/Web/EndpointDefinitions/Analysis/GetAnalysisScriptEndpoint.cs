using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.Analysis;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Analysis;

public class GetAnalysisScriptEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetAnalysisScriptUseCase, GetAnalysisScriptService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/projects/{projectId}/analyses/{analysisId}/scripts/{scriptId}/content",
                GetScriptContent
            )
            .WithTags("Analysis")
            .WithSummary("Get analysis script content")
            .WithDescription(
                "Retrieves the actual code content of a script used in a specific analysis."
            );

        app.MapGet(
                "/api/projects/{projectId}/analyses/{analysisId}/scripts/{scriptId}",
                GetScriptMetadata
            )
            .WithTags("Analysis")
            .WithSummary("Get analysis script metadata")
            .WithDescription(
                "Retrieves metadata about a script used in a specific analysis, such as its name and type."
            );
    }

    private static async Task<
        Results<ContentHttpResult, NotFound<ProblemDetails>>
    > GetScriptContent(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        [FromRoute] string scriptId,
        IGetAnalysisScriptUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.GetScriptContent(
            new AnalysisId(analysisId),
            new ScriptId(scriptId),
            cancellationToken
        );

        return result.Match<Results<ContentHttpResult, NotFound<ProblemDetails>>>(
            content => TypedResults.Text(content),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }

    private static async Task<
        Results<Ok<WebScriptData>, NotFound<ProblemDetails>>
    > GetScriptMetadata(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        [FromRoute] string scriptId,
        IGetAnalysisScriptUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.GetFileScript(
            new AnalysisId(analysisId),
            new ScriptId(scriptId),
            cancellationToken
        );

        return result.Match<Results<Ok<WebScriptData>, NotFound<ProblemDetails>>>(
            script => TypedResults.Ok(WebScriptData.Map(script)),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }
}
