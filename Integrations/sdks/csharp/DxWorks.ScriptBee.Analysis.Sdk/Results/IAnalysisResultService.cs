using ScriptBee.Domain.Model.Analysis;

namespace DxWorks.ScriptBee.Analysis.Sdk.Results;

public interface IAnalysisResultService
{
    Task<ResultId> AddFileAsync(
        string name,
        Stream content,
        string resultType = RunResultTypes.File,
        CancellationToken cancellationToken = default
    );

    Task<ResultId> AddFileAsync(
        string name,
        string content,
        string resultType = RunResultTypes.File,
        CancellationToken cancellationToken = default
    );

    Task<ResultId> AddConsoleAsync(
        string content,
        string name = "ConsoleOutput",
        CancellationToken cancellationToken = default
    );

    Task<ResultId> AddErrorAsync(
        string message,
        string name = "RunError",
        CancellationToken cancellationToken = default
    );

    Task<ResultId> AddResultAsync(
        string name,
        string resultType,
        Stream content,
        CancellationToken cancellationToken = default
    );
}
