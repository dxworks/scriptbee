using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Entity;

[BsonIgnoreExtraElements]
public class MongodbProjectToken : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public required string ProjectId { get; init; }
    public required string TokenHash { get; init; }
    public string? Description { get; init; }

    public required string Role { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }

    public ProjectToken ToProjectToken()
    {
        return new ProjectToken(
            new ProjectTokenId(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId),
            TokenHash,
            Description,
            new UserRole(Role),
            CreatedAt,
            ExpiresAt
        );
    }
}
