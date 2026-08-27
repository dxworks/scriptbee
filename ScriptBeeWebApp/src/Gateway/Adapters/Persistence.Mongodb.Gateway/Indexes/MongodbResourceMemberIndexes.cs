using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Indexes;

public class MongodbResourceMemberIndexes(IMongoRepository<MongodbResourceMember> mongoRepository)
    : IIndexCreator
{
    public async Task Create(CancellationToken cancellationToken)
    {
        var index = new CreateIndexModel<MongodbResourceMember>(
            Builders<MongodbResourceMember>
                .IndexKeys.Ascending(x => x.ResourceType)
                .Ascending(x => x.ResourceId)
                .Ascending(x => x.MemberType)
                .Ascending(x => x.MemberId)
        );

        await mongoRepository.MongoCollection.Indexes.CreateOneAsync(
            index,
            cancellationToken: cancellationToken
        );
    }
}
