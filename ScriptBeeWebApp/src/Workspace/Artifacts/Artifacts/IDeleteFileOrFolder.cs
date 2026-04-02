using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public interface IDeleteFileOrFolder
{
    void Delete(ProjectId projectId, string path);
}
