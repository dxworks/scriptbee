using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Analysis;

public class GetAnalysisStatusEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetAnalysisUseCase, GetAnalysisService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/analyses", GetAllAnalysis).WithTags("Analysis");
        app.MapGet("/api/projects/{projectId}/analyses/{analysisId}", GetAnalysisById)
            .WithTags("Analysis");
    }

    private static async Task<
        Results<Ok<WebAnalysisListResponse>, NotFound<ProblemDetails>>
    > GetAllAnalysis(
        HttpContext context,
        [FromRoute] string projectId,
        IGetAnalysisUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        // TODO FIXIT(#105): update sort parsing
        var result = await useCase.GetAll(
            ProjectId.FromValue(projectId),
            SortOrder.Descending,
            cancellationToken
        );
        return TypedResults.Ok(
            new WebAnalysisListResponse(result.Select(WebAnalysisInfo.Map).ToList())
        );
    }

    private static async Task<
        Results<Accepted<WebAnalysisInfo>, Ok<WebAnalysisInfo>, NotFound<ProblemDetails>>
    > GetAnalysisById(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        IGetAnalysisUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetById(new AnalysisId(analysisId), cancellationToken);

        return result.Match<
            Results<Accepted<WebAnalysisInfo>, Ok<WebAnalysisInfo>, NotFound<ProblemDetails>>
        >(
            analysis =>
            {
                if (analysis.IsRunning())
                {
                    return TypedResults.Accepted(
                        $"/api/projects/{projectId}/analyses/{analysisId}",
                        WebAnalysisInfo.Map(analysis)
                    );
                }

                return TypedResults.Ok(WebAnalysisInfo.Map(analysis));
            },
            error => error.ToProblem(context)
        );
    }
}
