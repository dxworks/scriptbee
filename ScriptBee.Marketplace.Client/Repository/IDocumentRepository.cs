namespace ScriptBee.Marketplace.Client.Repository;

public interface IDocumentRepository<T>
{
    Task<T> GetDocumentAsync(string collection, string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetAllDocumentsAsync(string collection, CancellationToken cancellationToken = default);
}
