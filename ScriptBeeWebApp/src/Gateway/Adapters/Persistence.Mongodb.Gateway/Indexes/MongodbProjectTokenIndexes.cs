using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Indexes;

public class MongodbProjectTokenIndexes(IMongoRepository<MongodbProjectToken> mongoRepository)
    : IIndexCreator
{
    public async Task Create(CancellationToken cancellationToken)
    {
        var index = new CreateIndexModel<MongodbProjectToken>(
            Builders<MongodbProjectToken>.IndexKeys.Ascending(x => x.TokenHash),
            new CreateIndexOptions { Unique = true }
        );

        await mongoRepository.MongoCollection.Indexes.CreateOneAsync(
            index,
            cancellationToken: cancellationToken
        );
    }
}
