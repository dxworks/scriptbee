using OneOf;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public class LoadFileAdapter(IConfigFoldersService configFoldersService) : ILoadFile
{
    public async Task<OneOf<string, FileDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        string path,
        CancellationToken cancellationToken = default
    )
    {
        var pathInSrcFolder = configFoldersService.GetPathToSrcFolder(projectId, path);

        if (!File.Exists(pathInSrcFolder))
        {
            return new FileDoesNotExistsError(path);
        }

        return await File.ReadAllTextAsync(pathInSrcFolder, cancellationToken);
    }
}

