namespace ScriptBee.Persistence.File.Plugin.Installer;

public interface IDownloadService
{
    Task DownloadFileAsync(
        string url,
        string filePath,
        CancellationToken cancellationToken = default
    );
}
