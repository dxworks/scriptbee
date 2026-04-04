using OneOf;
using ScriptBee.Application.Model.Pagination;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts;

public interface IGetScripts
{
    Task<IEnumerable<Script>> GetAll(ProjectId projectId, CancellationToken cancellationToken);

    Task<OneOf<Script, ScriptDoesNotExistsError>> Get(
        ScriptId scriptId,
        CancellationToken cancellationToken
    );

    Task<Page<ProjectStructureEntry>> ListRootEntries(
        ProjectId projectId,
        int offset,
        int limit,
        CancellationToken cancellationToken
    );

    Task<OneOf<Page<ProjectStructureEntry>, ScriptDoesNotExistsError>> ListEntries(
        ProjectId projectId,
        ScriptId scriptId,
        int offset,
        int limit,
        CancellationToken cancellationToken
    );
}
