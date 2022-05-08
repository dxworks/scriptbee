using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ScriptBee.Models;

public class ProjectModel : IDocument
{
    [BsonId]
    public string Id { get; set; }

    public string Name { get; set; }
    public DateTime CreationDate { get; set; }
    public Dictionary<string, List<string>> SavedFiles { get; set; } = new();
    public Dictionary<string, List<string>> LoadedFiles { get; set; } = new();
    public List<string> Loaders { get; set; } = new();
    public string Linker { get; set; }
    
    public int LastRunIndex { get; set; }
}