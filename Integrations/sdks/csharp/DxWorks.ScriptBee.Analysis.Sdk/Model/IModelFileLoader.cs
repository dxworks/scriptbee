using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Model;

public interface IModelFileLoader
{
    Task<Stream> LoadModelFileStreamAsync(FileId fileId, CancellationToken cancellationToken);
}
