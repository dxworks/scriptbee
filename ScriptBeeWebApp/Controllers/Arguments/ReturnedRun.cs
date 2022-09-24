using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.Arguments;

public record ReturnedRun(
    string RunId,
    long RunIndex,
    string ProjectId
)
{
    public List<OutputResult> Results { get; set; } = new();
}

public record OutputResult(
    string OutputId,
    string OutputType,
    string Path
);
