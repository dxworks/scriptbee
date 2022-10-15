using System;
using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.DTO;

public record ReturnedRun(int Index, string ScriptName, string Linker)
{
    public Dictionary<string, List<string>> LoadedFiles { get; set; } = new();

    public List<Result> Results { get; set; } = new();
}

public record Result(Guid Id, string Type, string Path);
