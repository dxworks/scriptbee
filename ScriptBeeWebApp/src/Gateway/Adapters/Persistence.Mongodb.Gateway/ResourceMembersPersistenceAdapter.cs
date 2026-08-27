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
) : IGetResourceRole, ISetResourceRole, IGetProjectMembers, IRemoveProjectMember
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

    public async Task<List<ProjectId>> GetAccessibleProjectIds(
        UserId userId,
        List<UserGroup> groups,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<MongodbResourceMember>.Filter.And(
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceType, ProjectResourceType),
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

        var members = await mongoRepository
            .MongoCollection.Find(filter)
            .ToListAsync(cancellationToken);

        return [.. members.Select(m => ProjectId.FromValue(m.ResourceId)).Distinct()];
    }

    public async Task SetRoleForUser(
        UserId userId,
        ProjectId projectId,
        UserRole role,
        CancellationToken cancellationToken
    )
    {
        await UpsertMemberRole(userId.Value, UserMemberType, projectId, role, cancellationToken);
    }

    public async Task SetRoleForMember(
        string memberId,
        string memberType,
        ProjectId projectId,
        UserRole role,
        CancellationToken cancellationToken
    )
    {
        await UpsertMemberRole(memberId, memberType, projectId, role, cancellationToken);
    }

    public async Task<List<ProjectMember>> GetProjectMembers(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<MongodbResourceMember>.Filter.And(
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceType, ProjectResourceType),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceId, projectId.Value)
        );

        var members = await mongoRepository
            .MongoCollection.Find(filter)
            .ToListAsync(cancellationToken);

        return
        [
            .. members.Select(m => new ProjectMember(
                m.MemberId,
                m.MemberType,
                new UserRole(m.Role)
            )),
        ];
    }

    public async Task RemoveProjectMember(
        ProjectId projectId,
        string memberId,
        string memberType,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<MongodbResourceMember>.Filter.And(
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceType, ProjectResourceType),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceId, projectId.Value),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.MemberId, memberId),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.MemberType, memberType)
        );

        await mongoRepository.MongoCollection.DeleteOneAsync(filter, cancellationToken);
    }

    private async Task UpsertMemberRole(
        string memberId,
        string memberType,
        ProjectId projectId,
        UserRole role,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<MongodbResourceMember>.Filter.And(
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceType, ProjectResourceType),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.ResourceId, projectId.Value),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.MemberType, memberType),
            Builders<MongodbResourceMember>.Filter.Eq(m => m.MemberId, memberId)
        );

        var update = Builders<MongodbResourceMember>
            .Update.SetOnInsert(m => m.ResourceType, ProjectResourceType)
            .SetOnInsert(m => m.ResourceId, projectId.Value)
            .SetOnInsert(m => m.MemberType, memberType)
            .SetOnInsert(m => m.MemberId, memberId)
            .SetOnInsert(m => m.AssignedAt, DateTimeOffset.UtcNow)
            .Set(m => m.Role, role.Value);

        await mongoRepository.MongoCollection.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken
        );
    }
}
