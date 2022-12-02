using System.Threading;
using System.Threading.Tasks;

namespace ScriptBeeWebApp.Services;

public interface IZipService
{
    Task UnzipFileAsync(string zipFilePath, string destinationPath, CancellationToken cancellationToken = default);
}
