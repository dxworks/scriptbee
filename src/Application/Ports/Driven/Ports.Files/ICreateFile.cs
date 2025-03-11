using OneOf;

namespace ScriptBee.Ports.Files;

public interface ICreateFile
{
    public Task<OneOf<CreateFileResult, FileAlreadyExistsError>> Create(
        string path,
        CancellationToken cancellationToken = default
    );
}
