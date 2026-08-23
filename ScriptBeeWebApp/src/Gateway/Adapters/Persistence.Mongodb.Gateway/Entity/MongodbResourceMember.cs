using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Entity;

[BsonIgnoreExtraElements]
public class MongodbResourceMember : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public required string ResourceType { get; init; }
    public required string ResourceId { get; init; }
    public required string MemberType { get; init; }
    public required string MemberId { get; init; }
    public required string Role { get; init; }
    public required DateTimeOffset AssignedAt { get; init; }
}
