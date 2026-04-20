namespace ScriptBee.Common.Extensions;

public static class FileSystemExtensions
{
    public static void DeleteIfExists(this DirectoryInfo directoryInfo, bool recursive = true)
    {
        if (directoryInfo.Exists)
        {
            directoryInfo.Delete(recursive);
        }
    }

    public static void DeleteIfExists(this FileInfo fileInfo)
    {
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
    }

    public static void EnsureDirectoryExists(this string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
