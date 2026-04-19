using OneOf;
using ScriptBee.Application.Model.Pagination;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.ProjectStructure;

namespace ScriptBee.Service.Gateway.ProjectStructure;

using GetResult = OneOf<
    Page<ProjectStructureEntry>,
    ProjectDoesNotExistsError,
    ScriptDoesNotExistsError
>;

public sealed class GetProjectFilesService(IGetProject getProject, IGetScripts getScripts)
    : IGetProjectFilesUseCase
{
    public async Task<GetResult> GetAll(
        GetProjectFilesQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(query.ProjectId, cancellationToken);

        return await result.Match<Task<GetResult>>(
            async _ => await GetFiles(query, cancellationToken),
            error => Task.FromResult<GetResult>(error)
        );
    }

    private async Task<GetResult> GetFiles(
        GetProjectFilesQuery query,
        CancellationToken cancellationToken
    )
    {
        if (query.ParentId is null)
        {
            return await getScripts.ListRootEntries(
                query.ProjectId,
                query.Offset,
                query.Limit,
                cancellationToken
            );
        }

        var result = await getScripts.ListEntries(
            query.ProjectId,
            (ScriptId)query.ParentId,
            query.Offset,
            query.Limit,
            cancellationToken
        );

        return result.Match<GetResult>(page => page, error => error);
    }
}
