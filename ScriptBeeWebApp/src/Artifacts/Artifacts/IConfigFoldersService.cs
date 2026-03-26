using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public interface IConfigFoldersService
{
    string GetPathToSrcFolder(ProjectId projectId, string path);

    string GetPathToUserFolder(string path);
}

