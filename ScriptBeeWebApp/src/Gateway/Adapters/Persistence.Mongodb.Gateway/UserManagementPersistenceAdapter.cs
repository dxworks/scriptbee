using MongoDB.Driver;
using ScriptBee.Domain.Model.User;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Persistence.Mongodb;

public sealed class UserManagementPersistenceAdapter(IMongoRepository<MongodbUser> mongoRepository)
    : IGetOrAddUser
{
    public async Task<UserId> GetOrAddUser(
        string externalUserId,
        string externalUserName,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<MongodbUser>.Filter.Eq(x => x.ExternalId, externalUserId);
        var update = Builders<MongodbUser>
            .Update.SetOnInsert(x => x.ExternalId, externalUserId)
            .SetOnInsert(x => x.Name, externalUserName)
            .SetOnInsert(x => x.CreatedAt, DateTimeOffset.UtcNow);

        var options = new FindOneAndUpdateOptions<MongodbUser>()
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After,
        };

        var user = await mongoRepository.MongoCollection.FindOneAndUpdateAsync(
            filter,
            update,
            options,
            cancellationToken
        );
        return new UserId(user.Id);
    }
}
