using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.Analysis;
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
            )
            .WithTags("Analysis")
            .WithSummary("Download individual analysis result file")
            .WithDescription("Downloads a specific file generated as a result of an analysis.");

        app.MapGet(
                "/api/projects/{projectId}/analyses/{analysisId}/results/files/download",
                DownloadAllFiles
            )
            .WithTags("Analysis")
            .WithSummary("Download all analysis result files")
            .WithDescription(
                "Downloads a ZIP archive containing all files generated as a result of an analysis."
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
            error => error.ToProblem(context)
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
