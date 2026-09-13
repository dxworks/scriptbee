using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface ILoadContextUseCase
{
    Task Load(
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken
    );
}
