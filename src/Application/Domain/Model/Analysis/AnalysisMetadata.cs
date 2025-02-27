namespace ScriptBee.Domain.Model.Analysis;

public record AnalysisMetadata(IEnumerable<string> Loaders, IEnumerable<string> Linkers);
