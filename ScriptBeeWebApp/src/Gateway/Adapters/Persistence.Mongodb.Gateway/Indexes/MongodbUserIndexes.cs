using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Indexes;

public class MongodbUserIndexes(IMongoRepository<MongodbUser> mongoRepository) : IIndexCreator
{
    public async Task Create(CancellationToken cancellationToken)
    {
        var index = new CreateIndexModel<MongodbUser>(
            Builders<MongodbUser>.IndexKeys.Ascending(x => x.ExternalId),
            new CreateIndexOptions { Unique = true }
        );

        await mongoRepository.MongoCollection.Indexes.CreateOneAsync(
            index,
            cancellationToken: cancellationToken
        );
    }
}
