using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api.Services;

public interface IHelperFunctionsResultService
{
    Task UploadResultAsync(string fileName, string type, string content, CancellationToken cancellationToken = default);

    Task UploadResultAsync(string fileName, string type, Stream content, CancellationToken cancellationToken = default);

    void UploadResult(string fileName, string type, string content);

    void UploadResult(string fileName, string type, Stream content);
}
