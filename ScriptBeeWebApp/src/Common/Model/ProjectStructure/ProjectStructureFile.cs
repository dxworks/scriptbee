namespace ScriptBee.Domain.Model.ProjectStructure;

public sealed record ProjectStructureFile(string Path)
{
    public readonly List<string> FilePathParts = Path.Replace("\\", "/").Split('/').ToList();

    public string Name => System.IO.Path.GetFileName(Path);
}
