using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ScriptBee.Models;

public class RunModel : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int RunIndex { get; set; }

    public string ProjectId { get; set; }
    public Dictionary<string, List<string>> LoadedFiles { get; set; } = new();
    public string Linker { get; set; }
    public string ScriptName { get; set; }
    public string ConsoleOutputName { get; set; }
    public List<string> OutputFileNames { get; set; } = new();
    public string? Errors { get; set; }
}