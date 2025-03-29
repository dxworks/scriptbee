using DxWorks.ScriptBee.Plugin.Api.Services;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

using GetConsoleResultType = OneOf<string, AnalysisDoesNotExistsError>;

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

    public async Task<
        OneOf<IEnumerable<AnalysisErrorResult>, AnalysisDoesNotExistsError>
    > GetErrorResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        return result.Match(analysisInfo => throw new NotImplementedException(), error => error);
    }

    public async Task<
        OneOf<IEnumerable<AnalysisFileResult>, AnalysisDoesNotExistsError>
    > GetFileResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        return result.Match(analysisInfo => throw new NotImplementedException(), error => error);
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
}
