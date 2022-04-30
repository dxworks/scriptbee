namespace HelperFunctions;

public record RunResult(string Type, string FilePath)
{
    public const string ConsoleType = "Console";
    public const string FileType = "File";
}