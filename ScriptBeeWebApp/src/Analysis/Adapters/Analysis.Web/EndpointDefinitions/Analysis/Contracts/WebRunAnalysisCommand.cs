using System.ComponentModel;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;

[Description("Command used to run an analysis script on the analysis service.")]
public record WebRunAnalysisCommand(string ProjectId, string ScriptId);
