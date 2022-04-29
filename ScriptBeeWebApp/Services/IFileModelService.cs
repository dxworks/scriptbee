using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptBeeWebApp.Services;

public interface IFileModelService
{
    public Task UploadFile(string fileName, Stream fileStream, CancellationToken cancellationToken);

    public Task<Stream> GetFile(string fileName);
}