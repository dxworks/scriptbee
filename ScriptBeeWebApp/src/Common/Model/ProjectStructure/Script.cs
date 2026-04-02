using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.ProjectStructure;

public record Script(
    ScriptId Id,
    ProjectId ProjectId,
    ProjectStructureFile File,
    ScriptLanguage ScriptLanguage,
    IEnumerable<ScriptParameter> Parameters
) : ProjectStructureEntry(Id, ProjectId, File);
