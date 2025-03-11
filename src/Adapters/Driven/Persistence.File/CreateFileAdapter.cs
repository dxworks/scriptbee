using OneOf;
using ScriptBee.Ports.Files;

namespace ScriptBee.Persistence.File;

public class CreateFileAdapter : ICreateFile
{
    public async Task<OneOf<CreateFileResult, FileAlreadyExistsError>> Create(
        string path,
        CancellationToken cancellationToken = default
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT(#32): finish implementation

        return new FileAlreadyExistsError(path);
    }
}
