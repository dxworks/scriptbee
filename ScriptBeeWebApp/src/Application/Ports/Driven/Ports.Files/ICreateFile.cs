using OneOf;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Files;

public interface ICreateFile
{
    Task<OneOf<CreateFileResult, FileAlreadyExistsError>> Create(
        ProjectId projectId,
        string path,
        string content,
        CancellationToken cancellationToken = default
    );
}
