using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Domain.Model.Errors;

public sealed record AnalysisResultDoesNotExistsError(ResultId Id);
