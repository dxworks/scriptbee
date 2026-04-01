using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public class UpdateFileAdapter(IConfigFoldersService configFoldersService) : IUpdateFile
{
    public async Task<OneOf<Success, FileDoesNotExistsError>> UpdateScriptContent(
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
}
