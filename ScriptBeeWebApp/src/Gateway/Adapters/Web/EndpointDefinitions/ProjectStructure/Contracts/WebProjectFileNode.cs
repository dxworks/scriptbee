using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public sealed record WebProjectFileNode(
    string Id,
    string Name,
    string Path,
    string AbsolutePath,
    string Type,
    bool HasChildren
)
{
    private const string File = "file";
    private const string Folder = "folder";

    public static WebProjectFileNode Map(ProjectStructureEntry entry, string absolutePath)
    {
        return entry switch
        {
            Script script => new WebProjectFileNode(
                script.Id.ToString(),
                script.File.Name,
                script.File.Path,
                absolutePath,
                File,
                false
            ),
            ScriptFolder scriptFolder => new WebProjectFileNode(
                scriptFolder.Id.ToString(),
                scriptFolder.File.Name,
                scriptFolder.File.Path,
                absolutePath,
                Folder,
                scriptFolder.ChildrenIds.Any()
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(entry)),
        };
    }
}
