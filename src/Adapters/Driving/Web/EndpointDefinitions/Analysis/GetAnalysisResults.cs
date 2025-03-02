using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Analysis;

public class GetAnalysisResults : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
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

    private static async Task<Ok<WebGetAnalysisResultConsole>> GetConsoleAnalysisResult(
        [FromRoute] string projectId,
        [FromRoute] string analysisId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value
        return TypedResults.Ok(new WebGetAnalysisResultConsole("test console output"));
    }

    private static async Task<Ok<WebGetAnalysisResultRunErrors>> GetErrorsAnalysisResult(
        [FromRoute] string projectId,
        [FromRoute] string analysisId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value
        return TypedResults.Ok(
            new WebGetAnalysisResultRunErrors(
                [new WebAnalysisResultRunError("Error", "There was an error", "Critical")]
            )
        );
    }

    private static async Task<Ok<WebGetAnalysisResultFileList>> GetFilesAnalysisResult(
        [FromRoute] string projectId,
        [FromRoute] string analysisId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            new WebGetAnalysisResultFileList(
                [new WebGetAnalysisResultFile("file-id", "File.csv", "file")]
            )
        );
    }
}
