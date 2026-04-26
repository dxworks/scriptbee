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

    public static void CopyTo(
        this DirectoryInfo sourceDir,
        string destinationDir,
        bool recursive = true
    )
    {
        if (!sourceDir.Exists)
        {
            throw new DirectoryNotFoundException(
                $"Source directory not found: {sourceDir.FullName}"
            );
        }

        Directory.CreateDirectory(destinationDir);

        foreach (var file in sourceDir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        if (!recursive)
        {
            return;
        }

        foreach (var subDir in sourceDir.GetDirectories())
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            subDir.CopyTo(newDestinationDir, true);
        }
    }
}
