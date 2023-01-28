using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

public sealed class ZipFileService : IZipFileService
{
    public Task UnzipFileAsync(string zipFilePath, string destinationPath,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => { ZipFile.ExtractToDirectory(zipFilePath, destinationPath, true); }, cancellationToken);
    }
}
