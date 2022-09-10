using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;

public class RelativeFileContentProvider : IFileContentProvider
{
    public Task<string> GetFileContentAsync(string path, CancellationToken cancellationToken = default)
    {
        return File.ReadAllTextAsync(
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, path), cancellationToken);
    }
}
