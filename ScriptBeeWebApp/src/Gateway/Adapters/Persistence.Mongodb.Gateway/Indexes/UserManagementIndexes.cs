using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Indexes;

public class UserManagementIndexes(IMongoRepository<MongodbUser> mongoRepository) : IIndexCreator
{
    public async Task Create(CancellationToken cancellationToken)
    {
        var indexKeys = Builders<MongodbUser>.IndexKeys.Ascending(x => x.ExternalId);

        var indexOptions = new CreateIndexOptions { Unique = true };

        var indexModel = new CreateIndexModel<MongodbUser>(indexKeys, indexOptions);

        await mongoRepository.MongoCollection.Indexes.CreateOneAsync(
            indexModel,
            cancellationToken: cancellationToken
        );
    }
}
