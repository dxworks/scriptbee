namespace ScriptBee.Gateway.Persistence.Mongodb.Repository;

public interface IMongoRepository<T> where T : IDocument
{
    public Task CreateDocument(T model, CancellationToken cancellationToken);

    public Task<T?> GetDocument(string id, CancellationToken cancellationToken);
    public Task<bool> DocumentExists(string id, CancellationToken cancellationToken);

    public Task<List<T>> GetAllDocuments(CancellationToken cancellationToken);

    public Task UpdateDocument(T model, CancellationToken cancellationToken);

    public Task DeleteDocument(string id, CancellationToken cancellationToken);
}
