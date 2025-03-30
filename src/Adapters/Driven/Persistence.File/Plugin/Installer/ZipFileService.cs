using System.IO.Compression;

namespace ScriptBee.Persistence.File.Plugin.Installer;

public sealed class ZipFileService : IZipFileService
{
    public Task UnzipFileAsync(
        string zipFilePath,
        string destinationPath,
        CancellationToken cancellationToken = default
    )
    {
        return Task.Run(
            () =>
            {
                ZipFile.ExtractToDirectory(zipFilePath, destinationPath, true);
            },
            cancellationToken
        );
    }
}
