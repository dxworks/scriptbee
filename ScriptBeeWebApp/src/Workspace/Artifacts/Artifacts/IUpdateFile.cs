using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public interface IUpdateFile
{
    Task<OneOf<Success, FileDoesNotExistsError>> UpdateScriptContent(
        ProjectId projectId,
        string path,
        string content,
        CancellationToken cancellationToken
    );
}
