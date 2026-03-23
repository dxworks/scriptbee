using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Analysis;

public record RunScriptRequest(
    IScriptRunner ScriptRunner,
    Script Script,
    AnalysisInfo AnalysisInfo
);
