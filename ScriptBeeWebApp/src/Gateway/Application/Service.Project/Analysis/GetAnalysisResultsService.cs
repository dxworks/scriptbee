using DxWorks.ScriptBee.Plugin.Api.Services;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

using GetConsoleResultType = OneOf<string, AnalysisDoesNotExistsError>;
using GetErrorResultType = OneOf<IEnumerable<AnalysisErrorResult>, AnalysisDoesNotExistsError>;
using GetFileResultType = OneOf<IEnumerable<AnalysisFileResult>, AnalysisDoesNotExistsError>;

public class GetAnalysisResultsService(IGetAnalysis getAnalysis, IFileModelService fileModelService)
    : IGetAnalysisResultsUseCase
{
    public async Task<GetConsoleResultType> GetConsoleResult(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        return await result.Match<Task<GetConsoleResultType>>(
            async analysisInfo => await GetConsoleContent(analysisInfo, cancellationToken),
            error => Task.FromResult<GetConsoleResultType>(error)
        );
    }

    public async Task<GetErrorResultType> GetErrorResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        return await result.Match<Task<GetErrorResultType>>(
            async analysisInfo => await GetErrorResults(analysisInfo, cancellationToken),
            error => Task.FromResult<GetErrorResultType>(error)
        );
    }

    public async Task<GetFileResultType> GetFileResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        return result.Match(GetFileResults, error => error);
    }

    private async Task<GetConsoleResultType> GetConsoleContent(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken
    )
    {
        var consoleResult = analysisInfo.Results.FirstOrDefault(r =>
            r.Type == RunResultDefaultTypes.ConsoleType
        );

        if (consoleResult == null)
        {
            return "";
        }

        var fileStream = await fileModelService.GetFileAsync(
            consoleResult.Id.ToFileId(),
            cancellationToken
        );
        using var reader = new StreamReader(fileStream);

        return await reader.ReadToEndAsync(cancellationToken);
    }

    private async Task<GetErrorResultType> GetErrorResults(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken
    )
    {
        var errorResults = analysisInfo
            .Results.Where(r => r.Type == RunResultDefaultTypes.RunError)
            .ToList();

        var analysisResults = new List<AnalysisErrorResult>(
            GetAnalysisErrorResultsFromAnalysisErrors(analysisInfo.Errors)
        );

        foreach (var resultSummary in errorResults)
        {
            var fileStream = await fileModelService.GetFileAsync(
                resultSummary.Id.ToFileId(),
                cancellationToken
            );
            using var reader = new StreamReader(fileStream);

            var message = await reader.ReadToEndAsync(cancellationToken);
            analysisResults.Add(
                new AnalysisErrorResult(resultSummary.Name, message, AnalysisErrorResult.Minor)
            );
        }

        return analysisResults;
    }

    private static GetFileResultType GetFileResults(AnalysisInfo analysisInfo)
    {
        var analysisFileResults = analysisInfo
            .Results.Where(r => r.Type == RunResultDefaultTypes.FileType)
            .Select(r => new AnalysisFileResult(r.Id.ToFileId(), r.Name, "file"));
        return GetFileResultType.FromT0(analysisFileResults);
    }

    private static IEnumerable<AnalysisErrorResult> GetAnalysisErrorResultsFromAnalysisErrors(
        IEnumerable<AnalysisError> analysisInfoErrors
    )
    {
        return analysisInfoErrors.Select(x => new AnalysisErrorResult(
            "Analysis Error",
            x.Message,
            AnalysisErrorResult.Major
        ));
    }
}
