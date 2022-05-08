using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.Arguments;

public record ReturnedRun
{
    public string RunId { get; set; }

    public int RunIndex { get; set; }

    public string ProjectId { get; set; }
    public string ConsoleOutputName { get; set; }

    public List<OutputFile> OutputFiles { get; set; } = new();
    public string? Errors { get; set; }
}

public record OutputFile(string FileName, string FileType, string FilePath);