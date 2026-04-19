using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Gateway.ProjectStructure;

public interface IDeleteProjectFilesUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError>> Delete(
        DeleteFileCommand command,
        CancellationToken cancellationToken
    );
}
