using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.ProjectStructure;

public abstract record ProjectStructureEntry(
    ScriptId Id,
    ProjectId ProjectId,
    ProjectStructureFile File
);
