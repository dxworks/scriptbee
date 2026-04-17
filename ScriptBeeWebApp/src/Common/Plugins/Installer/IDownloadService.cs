namespace ScriptBee.Plugins.Installer;

public interface IDownloadService
{
    Task DownloadFileAsync(
        string url,
        string filePath,
        CancellationToken cancellationToken = default
    );
}
