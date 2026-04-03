using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

public sealed class GetProjectFilesService : IGetProjectFilesUseCase
{
    public Task<OneOf<GetProjectFilesQueryResult, ProjectDoesNotExistsError>> GetAll(
        GetProjectFilesQuery query,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
