namespace DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

public record RunResult(string Type, string FilePath)
{
    public const string ConsoleType = "Console";
    public const string FileType = "File";
    public const string UIType = "UI";
}
