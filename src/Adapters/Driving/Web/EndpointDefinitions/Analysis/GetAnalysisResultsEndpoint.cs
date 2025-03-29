using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Analysis;

public class GetAnalysisResultsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetAnalysisResultsUseCase, GetAnalysisResultsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/projects/{projectId}/analyses/{analysisId}/results/console",
            GetConsoleAnalysisResult
        );
        app.MapGet(
            "/api/projects/{projectId}/analyses/{analysisId}/results/errors",
            GetErrorsAnalysisResult
        );
        app.MapGet(
            "/api/projects/{projectId}/analyses/{analysisId}/results/files",
            GetFilesAnalysisResult
        );
        // TODO FIXIT: add endpoints for download result, download all files, download all results(console,output-errors,files)
    }

    private static async Task<
        Results<Ok<WebGetAnalysisResultConsole>, NotFound<ProblemDetails>>
    > GetConsoleAnalysisResult(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        IGetAnalysisResultsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetConsoleResult(
            ProjectId.FromValue(projectId),
            new AnalysisId(analysisId),
            cancellationToken
        );

        return result.Match<Results<Ok<WebGetAnalysisResultConsole>, NotFound<ProblemDetails>>>(
            content => TypedResults.Ok(new WebGetAnalysisResultConsole(content)),
            error =>
                TypedResults.NotFound(
                    context.ToProblemDetails(
                        "Analysis Not Found",
                        $"An analysis with the ID '{error.Id.Value}' does not exists."
                    )
                )
        );
    }

    private static async Task<
        Results<Ok<WebGetAnalysisResultRunErrors>, NotFound<ProblemDetails>>
    > GetErrorsAnalysisResult(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        IGetAnalysisResultsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetErrorResults(
            ProjectId.FromValue(projectId),
            new AnalysisId(analysisId),
            cancellationToken
        );

        return result.Match<Results<Ok<WebGetAnalysisResultRunErrors>, NotFound<ProblemDetails>>>(
            results => TypedResults.Ok(WebGetAnalysisResultRunErrors.Map(results)),
            error =>
                TypedResults.NotFound(
                    context.ToProblemDetails(
                        "Analysis Not Found",
                        $"An analysis with the ID '{error.Id.Value}' does not exists."
                    )
                )
        );
    }

    private static async Task<
        Results<Ok<WebGetAnalysisResultFileList>, NotFound<ProblemDetails>>
    > GetFilesAnalysisResult(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        IGetAnalysisResultsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetFileResults(
            ProjectId.FromValue(projectId),
            new AnalysisId(analysisId),
            cancellationToken
        );

        return result.Match<Results<Ok<WebGetAnalysisResultFileList>, NotFound<ProblemDetails>>>(
            files => TypedResults.Ok(WebGetAnalysisResultFileList.Map(files)),
            error =>
                TypedResults.NotFound(
                    context.ToProblemDetails(
                        "Analysis Not Found",
                        $"An analysis with the ID '{error.Id.Value}' does not exists."
                    )
                )
        );
    }
}
