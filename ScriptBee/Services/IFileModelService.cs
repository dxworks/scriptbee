using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.Services;

public interface IFileModelService
{
    public Task UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default);

    public void UploadFile(string fileName, Stream fileStream);

    public Task<Stream> GetFileAsync(string fileName);

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
}
