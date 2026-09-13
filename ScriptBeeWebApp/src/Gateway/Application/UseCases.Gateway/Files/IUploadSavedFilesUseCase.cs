using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;

namespace ScriptBee.UseCases.Gateway.Files;

public interface IUploadSavedFilesUseCase
{
    Task<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>> Upload(
        UploadSavedFilesCommand command,
        CancellationToken cancellationToken = default
    );
}
