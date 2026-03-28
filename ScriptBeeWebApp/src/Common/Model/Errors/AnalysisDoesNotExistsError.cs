using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Domain.Model.Errors;

public sealed record AnalysisDoesNotExistsError(AnalysisId Id);
