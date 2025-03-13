using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Files;

namespace ScriptBee.Persistence.File;

public class LoadFileAdapter(IConfigFoldersService configFoldersService) : ILoadFile
{
    public async Task<OneOf<string, FileDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        string path,
        CancellationToken cancellationToken = default
    )
    {
        var pathInSrcFolder = configFoldersService.GetPathToSrcFolder(projectId, path);

        if (!System.IO.File.Exists(pathInSrcFolder))
        {
            return new FileDoesNotExistsError(path);
        }

        return await System.IO.File.ReadAllTextAsync(pathInSrcFolder, cancellationToken);
    }
}
