using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.ProjectStructure;

public record CreateScriptCommand(
    ProjectId ProjectId,
    string Path,
    string Language,
    IEnumerable<ScriptParameter> Parameters
);
