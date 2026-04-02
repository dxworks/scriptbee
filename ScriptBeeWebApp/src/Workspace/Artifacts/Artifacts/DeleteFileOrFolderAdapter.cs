using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public class DeleteFileOrFolderAdapter(IConfigFoldersService configFoldersService)
    : IDeleteFileOrFolder
{
    public void Delete(ProjectId projectId, string path)
    {
        var pathInSrcFolder = configFoldersService.GetPathToSrcFolder(projectId, path);

        if (File.Exists(pathInSrcFolder))
        {
            File.Delete(pathInSrcFolder);
            return;
        }

        if (Directory.Exists(pathInSrcFolder))
        {
            Directory.Delete(pathInSrcFolder, true);
        }
    }
}
