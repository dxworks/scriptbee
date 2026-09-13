using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Results;

public sealed class ScriptResultsStore(IFileModelService fileModelService) : IScriptResultsStore
{
    public async Task UploadFileAsync<TMetadata>(
        FileId fileId,
        Stream fileStream,
        TMetadata? metadata = null,
        CancellationToken cancellationToken = default
    )
        where TMetadata : class
    {
        await fileModelService.UploadFileAsync(fileId, fileStream, metadata, cancellationToken);
    }
}
