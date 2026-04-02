using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface IDeleteProjectFilesUseCase
{
    Task<OneOf<Success, ProjectDoesNotExistsError>> Delete(
        DeleteFileCommand command,
        CancellationToken cancellationToken
    );
}
