using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;

public interface IFileContentProvider
{
    Task<string> GetFileContentAsync(string path, CancellationToken cancellationToken = default);
}
