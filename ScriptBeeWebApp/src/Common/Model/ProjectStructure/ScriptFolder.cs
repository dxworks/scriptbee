using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.ProjectStructure;

public record ScriptFolder(
    ScriptId Id,
    ProjectId ProjectId,
    ProjectStructureFile File,
    IEnumerable<ScriptId> ChildrenIds
) : ProjectStructureEntry(Id, ProjectId, File);
