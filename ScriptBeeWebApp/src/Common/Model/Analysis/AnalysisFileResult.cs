using ScriptBee.Domain.Model.File;

namespace ScriptBee.Domain.Model.Analysis;

public record AnalysisFileResult(FileId Id, string Name, string Type);
