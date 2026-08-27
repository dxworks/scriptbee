using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Indexes;

public class MongodbResourceMemberIndexes(IMongoRepository<MongodbResourceMember> mongoRepository)
    : IIndexCreator
{
    public async Task Create(CancellationToken cancellationToken)
    {
        var indexes = new[]
        {
            new CreateIndexModel<MongodbResourceMember>(
                Builders<MongodbResourceMember>
                    .IndexKeys.Ascending(x => x.ResourceType)
                    .Ascending(x => x.ResourceId)
                    .Ascending(x => x.MemberType)
                    .Ascending(x => x.MemberId)
            ),
            new CreateIndexModel<MongodbResourceMember>(
                Builders<MongodbResourceMember>
                    .IndexKeys.Ascending(x => x.ResourceType)
                    .Ascending(x => x.MemberType)
                    .Ascending(x => x.MemberId)
                    .Ascending(x => x.ResourceId)
            ),
        };

        await mongoRepository.MongoCollection.Indexes.CreateManyAsync(indexes, cancellationToken);
    }
}
