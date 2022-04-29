using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ScriptBeeWebApp.Controllers.Arguments;

public record ReturnedProject
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }
    public List<ReturnedNode> SavedFiles { get; set; } = new();
    public List<ReturnedNode> LoadedFiles { get; set; } = new();
    public List<string> Loaders { get; set; } = new();
    public string? Linker { get; set; }
}

public record ReturnedNode(string LoaderName, List<string> Files);