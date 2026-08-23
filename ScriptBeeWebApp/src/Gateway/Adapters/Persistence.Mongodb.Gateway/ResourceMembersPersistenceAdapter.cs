using MongoDB.Driver;
using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Persistence.Mongodb;

public sealed class ResourceMembersPersistenceAdapter(
    IMongoRepository<MongodbResourceMember> mongoRepository
) : IGetResourceRole, ISetResourceRole
{
    private const string ProjectResourceType = "project";
    private const string UserMemberType = "user";
    private const string GroupMemberType = "group";

    public async Task<UserRole?> GetRole(
        UserId userId,
        List<UserGroup> groups,
        OneOf<ProjectId> resourceId,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<MongodbResourceMember>.Filter.And(
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceType, ProjectResourceType),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceId, resourceId.AsT0.Value),
            Builders<MongodbResourceMember>.Filter.Or(
                Builders<MongodbResourceMember>.Filter.And(
                    Builders<MongodbResourceMember>.Filter.Eq(m => m.MemberType, UserMemberType),
                    Builders<MongodbResourceMember>.Filter.Eq(m => m.MemberId, userId.Value)
                ),
                Builders<MongodbResourceMember>.Filter.And(
                    Builders<MongodbResourceMember>.Filter.Eq(m => m.MemberType, GroupMemberType),
                    Builders<MongodbResourceMember>.Filter.In(
                        m => m.MemberId,
                        [.. groups.Select(g => g.Value)]
                    )
                )
            )
        );

        var resourceMember = await mongoRepository
            .MongoCollection.Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        if (resourceMember == null)
        {
            return null;
        }

        return new UserRole(resourceMember.Role);
    }

    public async Task SetRoleForUser(
        UserId userId,
        ProjectId projectId,
        UserRole role,
        CancellationToken cancellationToken
    )
    {
        var model = new MongodbResourceMember
        {
            Id = null!,
            ResourceType = ProjectResourceType,
            ResourceId = projectId.Value,
            MemberType = UserMemberType,
            MemberId = userId.Value,
            Role = role.Value,
            AssignedAt = DateTimeOffset.UtcNow,
        };

        await mongoRepository.CreateDocument(model, cancellationToken);
    }
}
