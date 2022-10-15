using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ScriptBee.Models;

public class RunModel : IDocument
{
    public List<Run> Runs { get; set; } = new();

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
}
