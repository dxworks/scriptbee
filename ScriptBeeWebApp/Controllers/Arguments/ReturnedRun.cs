using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.Arguments;

public record ReturnedRun
{
    public string Id { get; set; }
    public string ConsoleOutputName { get; set; }
    public List<string> OutputFileNames { get; set; } = new();
    public string? Errors { get; set; }
}