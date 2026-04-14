namespace ScriptBee.Rest.Contracts;

public record RestGenerateClasses(List<string> Languages, string? TransferFormat = null);
