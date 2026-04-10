using static System.IO.Path;

namespace ScriptBee.Domain.Model.ProjectStructure;

public sealed record ProjectStructureFile(string Path)
{
    public readonly string? ParentPath = GetDirectoryName(Path)?.Replace("\\", "/");
    public string Name => GetFileName(Path);

    public ProjectStructureFile UpdateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        var normalized = name.Replace('\\', '/');

        var fileName = GetFileName(normalized);

        if (fileName == "." || fileName == ".." || string.IsNullOrEmpty(fileName))
        {
            return this;
        }

        var directory = GetDirectoryName(Path);

        var newPath = string.IsNullOrEmpty(directory) ? fileName : Combine(directory, fileName);

        newPath = newPath.Replace('\\', '/');
        return new ProjectStructureFile(newPath);
    }
}
