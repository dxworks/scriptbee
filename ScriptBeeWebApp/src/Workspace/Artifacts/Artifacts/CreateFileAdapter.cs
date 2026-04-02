using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts;

public class CreateFileAdapter(IConfigFoldersService configFoldersService) : ICreateFile
{
    public async Task<OneOf<ProjectStructureFile, FileAlreadyExistsError>> Create(
        ProjectId projectId,
        string path,
        string content,
        CancellationToken cancellationToken
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

        return new ProjectStructureFile(path);
    }
}
