using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.Arguments;

public record ReturnedRun(
    string RunId,
    long RunIndex,
    string ProjectId,
    string ConsoleOutputName,
    string? Errors
)
{
    public List<OutputFile> OutputFiles { get; set; } = new();
}

public record OutputFile(string FileName, string FileType, string FilePath);
