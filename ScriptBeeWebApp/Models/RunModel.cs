﻿using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Models;

public class RunModel : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string ProjectId { get; set; }
    public Dictionary<string, List<string>> LoadedFiles { get; set; } = new();
    public string Linker { get; set; }
    public string ScriptId { get; set; }
    public string ConsoleOutput { get; set; }
    public List<string> OutputFilesId { get; set; }
}