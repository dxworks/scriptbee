using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public interface IUpdateFile
{
    Task<OneOf<Success, FileDoesNotExistsError>> UpdateContent(
        ProjectId projectId,
        string path,
        string content,
        CancellationToken cancellationToken
    );

    OneOf<Success, FileDoesNotExistsError, FileAlreadyExistsError> RenameFile(
        ProjectId projectId,
        string oldPath,
        string newPath
    );
}
