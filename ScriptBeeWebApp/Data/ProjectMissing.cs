namespace ScriptBeeWebApp.Data;

public record ProjectMissing(string ProjectId, string Message = "Project not found");
