using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

public interface IFileModelService
{
    // todo maybe combine with IResultCollector
    public Task UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default);

    public void UploadFile(string fileName, Stream fileStream);

    public Task<Stream> GetFileAsync(string fileName);

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
}
