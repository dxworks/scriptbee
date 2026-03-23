using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Files;

public interface IConfigFoldersService
{
    string GetPathToSrcFolder(ProjectId projectId, string path);

    string GetPathToUserFolder(string path);
}
