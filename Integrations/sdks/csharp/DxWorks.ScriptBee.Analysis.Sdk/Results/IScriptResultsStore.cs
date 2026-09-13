using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Results;

public interface IScriptResultsStore
{
    Task UploadFileAsync<TMetadata>(
        FileId fileId,
        Stream fileStream,
        TMetadata? metadata = null,
        CancellationToken cancellationToken = default
    )
        where TMetadata : class;
}
