namespace ScriptBeeWebApp.Data;

public record InvalidScriptType(string ScriptType, string Message = "Invalid script type");
