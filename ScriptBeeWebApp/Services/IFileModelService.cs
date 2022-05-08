using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptBeeWebApp.Services;

public interface IFileModelService
{
    public Task UploadFile(string fileName, Stream fileStream, CancellationToken cancellationToken = default);

    public Task<Stream> GetFile(string fileName);

    public Task DeleteFile(string fileName, CancellationToken cancellationToken = default);
}