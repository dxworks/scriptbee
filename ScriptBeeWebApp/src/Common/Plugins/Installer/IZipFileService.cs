namespace ScriptBee.Common.Plugins.Installer;

public interface IZipFileService
{
    Task UnzipFileAsync(
        string zipFilePath,
        string destinationPath,
        CancellationToken cancellationToken = default
    );
}
