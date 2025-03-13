using OneOf;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Files;

public interface ILoadFile
{
    Task<OneOf<string, FileDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        string path,
        CancellationToken cancellationToken = default
    );
}
