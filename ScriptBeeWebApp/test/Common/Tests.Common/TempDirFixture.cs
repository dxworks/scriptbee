namespace ScriptBee.Tests.Common;

public class TempDirFixture : IAsyncLifetime
{
    private string FullPath { get; set; } = null!;

    public ValueTask InitializeAsync()
    {
        FullPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}");
        Directory.CreateDirectory(FullPath);
        return ValueTask.CompletedTask;
    }

    public string CreateSubFolder(string name)
    {
        var path = Path.Combine(FullPath, name);
        Directory.CreateDirectory(path);
        return path;
    }

    public ValueTask DisposeAsync()
    {
        if (Directory.Exists(FullPath))
        {
            Directory.Delete(FullPath, true);
        }
        return ValueTask.CompletedTask;
    }
}
