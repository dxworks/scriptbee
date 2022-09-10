namespace ScriptBeeWebApp.Dto;

public record GenerateScriptRequest(
    string ProjectId,
    string ScriptType
);
