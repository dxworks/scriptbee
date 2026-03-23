using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Domain.Model.Errors;

public record AnalysisDoesNotExistsError(AnalysisId Id);
