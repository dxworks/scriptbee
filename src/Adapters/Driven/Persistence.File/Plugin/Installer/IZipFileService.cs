namespace ScriptBee.Persistence.File.Plugin.Installer;

public interface IZipFileService
{
    Task UnzipFileAsync(string zipFilePath, string destinationPath, CancellationToken cancellationToken = default);
}
