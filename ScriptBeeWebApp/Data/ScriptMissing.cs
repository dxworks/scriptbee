namespace ScriptBeeWebApp.Data;

public record ScriptMissing(string ScriptId, string Message = "Script not found");
