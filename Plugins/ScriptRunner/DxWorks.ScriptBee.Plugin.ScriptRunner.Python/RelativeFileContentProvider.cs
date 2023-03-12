using System.Reflection;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python;

internal static class RelativeFileContentProvider
{
    public static Task<string> GetFileContentAsync(string path, CancellationToken cancellationToken = default)
    {
        return File.ReadAllTextAsync(
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, path), cancellationToken);
    }
}
