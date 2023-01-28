using System.Threading;
using System.Threading.Tasks;

namespace ScriptBee.FileManagement;

public interface IDownloadService
{
    Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken = default); 
}
