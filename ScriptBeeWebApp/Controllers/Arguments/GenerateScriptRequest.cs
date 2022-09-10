namespace ScriptBeeWebApp.Controllers.Arguments;

public record GenerateScriptRequest(
    string ProjectId,
    string ScriptType
);
