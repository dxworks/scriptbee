using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.FileManagement;

public sealed class DownloadService : IDownloadService
{
    private readonly HttpClient _httpClient;

    public DownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            using var s = _httpClient.GetStreamAsync(url, cancellationToken);
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
            s.Result.CopyTo(fs);
        }, cancellationToken);
    }
}
