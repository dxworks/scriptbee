using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Gateway.ProjectStructure;

public record UpdateScriptCommand(
    ProjectId ProjectId,
    ScriptId ScriptId,
    string? Name,
    IEnumerable<ScriptParameter>? Parameters
);
