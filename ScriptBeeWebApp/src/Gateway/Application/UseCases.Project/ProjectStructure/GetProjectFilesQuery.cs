using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public sealed record GetProjectFilesQuery(ProjectId Id, ScriptId? parentId, int offset, int limit);

public sealed record GetProjectFilesQueryResult(
    IEnumerable<ProjectStructureEntry> Data,
    int TotalCount,
    int Offset,
    int Limit
);
