using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Application.Model;
using ScriptBee.Application.Model.Sorting;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.Analysis;
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
        app.MapGet("/api/projects/{projectId}/analyses", GetAllAnalysis)
            .WithTags("Analysis")
            .WithSummary("Get all analyses for a project")
            .WithDescription(
                "Retrieves a list of all analyses performed within the specified project."
            );
        app.MapGet("/api/projects/{projectId}/analyses/{analysisId}", GetAnalysisById)
            .WithTags("Analysis")
            .WithSummary("Get analysis status by ID")
            .WithDescription("Retrieves the current status and details of a specific analysis.");
    }

    private static async Task<
        Results<Ok<WebAnalysisListResponse>, NotFound<ProblemDetails>>
    > GetAllAnalysis(
        HttpContext context,
        [FromRoute] string projectId,
        IGetAnalysisUseCase useCase,
        [FromQuery] string? sort = "-CreationDate",
        CancellationToken cancellationToken = default
    )
    {
        var sorts = SortParser
            .Parse<AnalysisSortField>(sort)
            .Select(x => new AnalysisSort(x.Field, x.Direction))
            .ToList();

        var result = await useCase.GetAll(
            new GetAnalysisQuery(ProjectId.FromValue(projectId), sorts),
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
