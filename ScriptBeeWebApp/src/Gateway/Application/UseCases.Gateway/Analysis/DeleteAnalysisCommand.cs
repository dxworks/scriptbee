using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Analysis;

public record DeleteAnalysisCommand(ProjectId ProjectId, AnalysisId AnalysisId);
