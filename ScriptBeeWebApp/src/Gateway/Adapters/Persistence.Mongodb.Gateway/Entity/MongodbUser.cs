using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Entity;

[BsonIgnoreExtraElements]
public class MongodbUser : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public required string ExternalId { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
}
