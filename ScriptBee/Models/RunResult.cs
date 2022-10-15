using System;

namespace ScriptBee.Models;

public class RunResult
{
    public Guid Id { get; set; }
    public string Path { get; set; } = "";
    public string Type { get; set; } = "";
}
