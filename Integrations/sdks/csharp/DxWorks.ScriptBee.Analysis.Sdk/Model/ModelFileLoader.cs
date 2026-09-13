using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Model;

public sealed class ModelFileLoader(IFileModelService fileModelService) : IModelFileLoader
{
    public async Task<Stream> LoadModelFileStreamAsync(
        FileId fileId,
        CancellationToken cancellationToken
    )
    {
        return await fileModelService.GetFileAsync(fileId, cancellationToken);
    }
}
