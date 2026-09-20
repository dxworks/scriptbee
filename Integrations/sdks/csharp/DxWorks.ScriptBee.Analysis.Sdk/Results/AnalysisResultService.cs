using System.Text;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;

namespace DxWorks.ScriptBee.Analysis.Sdk.Results;

public sealed class AnalysisResultService(
    IScriptResultsStore store,
    IGuidProvider guidProvider,
    IDateTimeProvider dateTimeProvider,
    Action<ResultSummary>? onResultAdded = null
) : IAnalysisResultService
{
    public Task<ResultId> AddFileAsync(
        string name,
        Stream content,
        string resultType = RunResultTypes.File,
        CancellationToken cancellationToken = default
    ) => AddResultAsync(name, resultType, content, cancellationToken);

    public async Task<ResultId> AddFileAsync(
        string name,
        string content,
        string resultType = RunResultTypes.File,
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return await AddResultAsync(name, resultType, stream, cancellationToken);
    }

    public async Task<ResultId> AddConsoleAsync(
        string content,
        string name = "ConsoleOutput",
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return await AddResultAsync(name, RunResultTypes.Console, stream, cancellationToken);
    }

    public async Task<ResultId> AddErrorAsync(
        string message,
        string name = "RunError",
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(message));
        return await AddResultAsync(name, RunResultTypes.RunError, stream, cancellationToken);
    }

    public async Task<ResultId> AddResultAsync(
        string name,
        string resultType,
        Stream content,
        CancellationToken cancellationToken = default
    )
    {
        var resultId = new ResultId(guidProvider.NewGuid());

        await store.UploadFileAsync<object>(resultId.ToFileId(), content, null, cancellationToken);

        onResultAdded?.Invoke(
            new ResultSummary(resultId, name, resultType, dateTimeProvider.UtcNow())
        );

        return resultId;
    }
}
