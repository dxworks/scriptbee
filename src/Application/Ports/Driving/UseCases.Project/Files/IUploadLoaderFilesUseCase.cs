using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;

namespace ScriptBee.UseCases.Project.Files;

public interface IUploadLoaderFilesUseCase
{
    Task<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>> Upload(
        UploadLoaderFilesCommand command,
        CancellationToken cancellationToken = default
    );
}
