using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Persistence.File;

public interface IConfigFoldersService
{
    string GetPathToSrcFolder(ProjectId projectId, string path);

    string GetPathToUserFolder(string path);
}
