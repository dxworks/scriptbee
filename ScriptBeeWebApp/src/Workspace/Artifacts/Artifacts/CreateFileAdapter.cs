using OneOf;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public class CreateFileAdapter(IConfigFoldersService configFoldersService) : ICreateFile
{
    public async Task<OneOf<CreateFileResult, FileAlreadyExistsError>> Create(
        ProjectId projectId,
        string path,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        var pathInSrcFolder = configFoldersService.GetPathToSrcFolder(projectId, path);

        if (File.Exists(pathInSrcFolder))
        {
            return new FileAlreadyExistsError(path);
        }

        Directory.CreateDirectory(
            Path.GetDirectoryName(pathInSrcFolder) ?? throw new InvalidOperationException()
        );
        await File.WriteAllTextAsync(pathInSrcFolder, content, cancellationToken);

        return new CreateFileResult(
            Path.GetFileName(path),
            path,
            configFoldersService.GetPathToUserFolder(pathInSrcFolder)
        );
    }
}
