using System.ComponentModel;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;

[Description("Command used to run an analysis script on the analysis service.")]
public record WebRunAnalysisCommand(string ProjectId, string ScriptId);
