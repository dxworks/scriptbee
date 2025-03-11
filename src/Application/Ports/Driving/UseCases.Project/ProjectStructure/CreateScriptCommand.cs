using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public record CreateScriptCommand(
    string Path,
    string Language,
    IEnumerable<ScriptParameter> Parameters
);
