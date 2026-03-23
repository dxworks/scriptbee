using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.ProjectStructure;

public record Script(
    ScriptId Id,
    ProjectId ProjectId,
    string Name,
    string FilePath,
    string AbsoluteFilePath,
    ScriptLanguage ScriptLanguage,
    IEnumerable<ScriptParameter> Parameters
);
