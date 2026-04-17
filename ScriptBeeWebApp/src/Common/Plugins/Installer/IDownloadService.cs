namespace ScriptBee.Common.Plugins.Installer;

public interface IDownloadService
{
    Task DownloadFileAsync(
        string url,
        string filePath,
        CancellationToken cancellationToken = default
    );
}
