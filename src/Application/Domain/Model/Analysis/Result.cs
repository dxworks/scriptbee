namespace ScriptBee.Domain.Model.Analysis;

public record Result(ResultId Id, string Type, Uri ContentUri, DateTimeOffset CreationDate);
