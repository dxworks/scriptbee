using ScriptBee.Domain.Model.File;

namespace ScriptBee.UseCases.Analysis;

public interface ILoadContextUseCase
{
    Task Load(
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken
    );
}
