using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.Services;

public interface IZipFileService
{
    Task UnzipFileAsync(string zipFilePath, string destinationPath, CancellationToken cancellationToken = default);
}
