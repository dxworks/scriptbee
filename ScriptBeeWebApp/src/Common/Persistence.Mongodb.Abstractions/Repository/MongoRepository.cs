using System.Linq.Expressions;
using MongoDB.Driver;

namespace ScriptBee.Persistence.Mongodb.Repository;

public class MongoRepository<T>(IMongoCollection<T> mongoCollection) : IMongoRepository<T>
    where T : IDocument
{
    public Task CreateDocument(T model, CancellationToken cancellationToken)
    {
        return mongoCollection.InsertOneAsync(model, cancellationToken: cancellationToken);
    }

    public async Task<T?> GetDocument(string id, CancellationToken cancellationToken)
    {
        return await GetDocument(x => x.Id == id, cancellationToken);
    }

    public async Task<T?> GetDocument(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        return await mongoCollection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllDocuments(CancellationToken cancellationToken)
    {
        return await mongoCollection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllDocuments(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        return await mongoCollection.Find(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllDocuments(
        FilterDefinition<T> filter,
        CancellationToken cancellationToken
    )
    {
        return await mongoCollection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllDocuments(
        Expression<Func<T, bool>> predicate,
        int offset,
        int limit,
        CancellationToken cancellationToken
    )
    {
        return await mongoCollection
            .Find(predicate)
            .Skip(offset)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountDocuments(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        return await mongoCollection.CountDocumentsAsync(predicate, null, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllDocuments(
        Expression<Func<T, bool>> predicate,
        SortDefinition<T>? sortDefinition,
        CancellationToken cancellationToken
    )
    {
        var findFluent = mongoCollection.Find(predicate);

        if (sortDefinition != null)
            findFluent = findFluent.Sort(sortDefinition);

        return await findFluent.ToListAsync(cancellationToken);
    }

    public async Task UpdateDocument(T model, CancellationToken cancellationToken)
    {
        await mongoCollection.ReplaceOneAsync(
            x => x.Id == model.Id,
            model,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken
        );
    }

    public async Task<T?> DeleteDocument(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);

        return await mongoCollection.FindOneAndDeleteAsync(
            filter,
            new FindOneAndDeleteOptions<T, T>(),
            cancellationToken
        );
    }

    public async Task DeleteDocument(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        await mongoCollection.DeleteOneAsync(predicate, cancellationToken);
    }
}
