using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Models;

public class FileModel : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string Name { get; set; }
    public string Content { get; set; }
}