namespace ScriptBee.Domain.Model.ProjectStructure;

public sealed record ProjectStructureFile(string Path)
{
    public readonly string? ParentPath = System.IO.Path.GetDirectoryName(Path)?.Replace("\\", "/");
    public string Name => System.IO.Path.GetFileName(Path);
}
