using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public sealed record GetProjectFilesQuery(
    ProjectId ProjectId,
    ScriptId? ParentId,
    int Offset,
    int Limit
);
