using System.ComponentModel;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

[Description("Command used to trigger a script analysis.")]
public record WebTriggerAnalysisCommand(string ScriptId);
