using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Analysis;

using DownloadResult = Results<FileStreamHttpResult, NotFound<ProblemDetails>>;

public class DownloadAnalysisFileResultsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<
            IDownloadAnalysisFileResultsUseCase,
            DownloadAnalysisFileResultsService
        >();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/projects/{projectId}/analyses/{analysisId}/results/files/{fileId}",
            DownloadIndividualFile
        );

        app.MapGet(
            "/api/projects/{projectId}/analyses/{analysisId}/results/files/download",
            DownloadAllFiles
        );
    }

    private static async Task<DownloadResult> DownloadIndividualFile(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        [FromRoute] string fileId,
        IDownloadAnalysisFileResultsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetFileResultStream(
            ProjectId.FromValue(projectId),
            new AnalysisId(analysisId),
            new ResultId(fileId),
            cancellationToken
        );

        return result.Match<DownloadResult>(
            namedStream =>
                TypedResults.File(
                    namedStream.Stream,
                    MediaTypeNames.Application.Octet,
                    namedStream.Name
                ),
            error => error.ToProblem(context),
            error =>
                TypedResults.NotFound(
                    context.ToProblemDetails(
                        "Result Not Found",
                        $"An analysis result with the ID '{error.Id.Value}' does not exists."
                    )
                )
        );
    }

    private static async Task<DownloadResult> DownloadAllFiles(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        IDownloadAnalysisFileResultsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetAllFilesZipStream(
            ProjectId.FromValue(projectId),
            new AnalysisId(analysisId),
            cancellationToken
        );

        return result.Match<DownloadResult>(
            namedStream =>
                TypedResults.File(
                    namedStream.Stream,
                    MediaTypeNames.Application.Octet,
                    namedStream.Name
                ),
            error => error.ToProblem(context)
        );
    }
}
