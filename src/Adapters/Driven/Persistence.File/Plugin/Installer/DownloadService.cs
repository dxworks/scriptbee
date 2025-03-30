namespace ScriptBee.Persistence.File.Plugin.Installer;

public sealed class DownloadService(HttpClient httpClient) : IDownloadService
{
    public Task DownloadFileAsync(string url, string filePath,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            using var s = httpClient.GetStreamAsync(url, cancellationToken);
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
            s.Result.CopyTo(fs);
        }, cancellationToken);
    }
}
