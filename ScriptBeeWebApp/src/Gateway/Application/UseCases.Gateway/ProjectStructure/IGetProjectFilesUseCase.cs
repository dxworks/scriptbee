using OneOf;
using ScriptBee.Application.Model.Pagination;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface IGetProjectFilesUseCase
{
    Task<
        OneOf<Page<ProjectStructureEntry>, ProjectDoesNotExistsError, ScriptDoesNotExistsError>
    > GetAll(GetProjectFilesQuery query, CancellationToken cancellationToken);
}
