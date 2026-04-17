namespace ScriptBee.Plugins.Installer;

public interface IZipFileService
{
    Task UnzipFileAsync(
        string zipFilePath,
        string destinationPath,
        CancellationToken cancellationToken = default
    );
}
