using OneOf;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Files;

public interface IUploadLoaderFilesUseCase
{
    Task<OneOf<IEnumerable<FileData>, ProjectDoesNotExistsError>> Upload(
        UploadLoaderFilesCommand command,
        CancellationToken cancellationToken = default
    );
}
