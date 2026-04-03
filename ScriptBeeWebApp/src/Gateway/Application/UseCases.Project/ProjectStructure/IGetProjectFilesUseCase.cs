using OneOf;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface IGetProjectFilesUseCase
{
    Task<OneOf<GetProjectFilesQueryResult, ProjectDoesNotExistsError>> GetAll(
        GetProjectFilesQuery query,
        CancellationToken cancellationToken
    );
}
