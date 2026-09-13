using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Gateway.Files;

public interface IDeleteSavedFileUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError, ProjectFileDoesNotExistsError>> Delete(
        DeleteSavedFileCommand command,
        CancellationToken cancellationToken = default
    );
}
