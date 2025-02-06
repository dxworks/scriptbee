using MongoDB.Driver;

namespace ScriptBee.Gateway.Persistence.Mongodb.Repository;

public class MongoRepository<T>(IMongoCollection<T> mongoCollection) : IMongoRepository<T>
    where T : IDocument
{
    public Task CreateDocument(T model, CancellationToken cancellationToken)
    {
        return mongoCollection.InsertOneAsync(model, cancellationToken: cancellationToken);
    }

    public async Task<T?> GetDocument(string id, CancellationToken cancellationToken)
    {
        var result = await mongoCollection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return result;
    }

    public async Task<bool> DocumentExists(string id, CancellationToken cancellationToken)
    {
        var document = await GetDocument(id, cancellationToken);
        return document != null;
    }

    public async Task<List<T>> GetAllDocuments(CancellationToken cancellationToken)
    {
        return await mongoCollection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task UpdateDocument(T model, CancellationToken cancellationToken)
    {
        await mongoCollection.ReplaceOneAsync(x => x.Id == model.Id, model, new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task DeleteDocument(string id, CancellationToken cancellationToken)
    {
        await mongoCollection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }
}
