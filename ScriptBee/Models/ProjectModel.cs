using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ScriptBee.Models;

public class ProjectModel : IDocument
{
    public string Name { get; set; } = "";
    public DateTime CreationDate { get; set; }
    public Dictionary<string, List<FileData>> SavedFiles { get; set; } = new();

    public Dictionary<string, List<FileData>> LoadedFiles { get; set; } = new();
    public string Linker { get; set; } = "";
    public Run? LastRun { get; set; }
    [BsonId] public string Id { get; set; }
}
