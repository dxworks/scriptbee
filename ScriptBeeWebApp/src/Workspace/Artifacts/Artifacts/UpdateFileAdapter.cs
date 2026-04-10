using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public class UpdateFileAdapter(IConfigFoldersService configFoldersService) : IUpdateFile
{
    public async Task<OneOf<Success, FileDoesNotExistsError>> UpdateContent(
        ProjectId projectId,
        string path,
        string content,
        CancellationToken cancellationToken
    )
    {
        var pathInSrcFolder = configFoldersService.GetPathToSrcFolder(projectId, path);

        if (!File.Exists(pathInSrcFolder))
        {
            return new FileDoesNotExistsError(path);
        }

        await File.WriteAllTextAsync(pathInSrcFolder, content, cancellationToken);

        return new Success();
    }

    public OneOf<Success, FileDoesNotExistsError, FileAlreadyExistsError> RenameFile(
        ProjectId projectId,
        string oldPath,
        string newPath
    )
    {
        var oldPathInSrcFolder = configFoldersService.GetPathToSrcFolder(projectId, oldPath);
        var newPathInSrcFolder = configFoldersService.GetPathToSrcFolder(projectId, newPath);

        if (!File.Exists(oldPathInSrcFolder))
        {
            return new FileDoesNotExistsError(oldPath);
        }

        if (File.Exists(newPathInSrcFolder))
        {
            return new FileAlreadyExistsError(newPath);
        }

        File.Move(oldPathInSrcFolder, newPathInSrcFolder);

        return new Success();
    }
}
