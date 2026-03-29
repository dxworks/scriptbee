using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Analysis;
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
        app.MapGet("/api/projects/{projectId}/analyses/{analysisId}", GetAnalysisStatus)
            .WithTags("Analysis");
    }

    private static async Task<
        Results<Accepted<WebAnalysisStatus>, Ok<WebAnalysisStatus>, NotFound<ProblemDetails>>
    > GetAnalysisStatus(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        IGetAnalysisUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetById(new AnalysisId(analysisId), cancellationToken);

        return result.Match<
            Results<Accepted<WebAnalysisStatus>, Ok<WebAnalysisStatus>, NotFound<ProblemDetails>>
        >(
            analysis =>
            {
                if (analysis.IsRunning())
                {
                    return TypedResults.Accepted(
                        $"/api/projects/{projectId}/analyses/{analysisId}",
                        WebAnalysisStatus.Map(analysis)
                    );
                }

                return TypedResults.Ok(WebAnalysisStatus.Map(analysis));
            },
            error => error.ToProblem(context)
        );
    }
}
