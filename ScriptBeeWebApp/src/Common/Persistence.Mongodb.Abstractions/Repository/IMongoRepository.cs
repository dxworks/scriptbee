using System.Linq.Expressions;
using MongoDB.Driver;

namespace ScriptBee.Persistence.Mongodb.Repository;

public interface IMongoRepository<T>
    where T : IDocument
{
    public Task CreateDocument(T model, CancellationToken cancellationToken);

    public Task<T?> GetDocument(string id, CancellationToken cancellationToken);

    public Task<T?> GetDocument(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task<IEnumerable<T>> GetAllDocuments(CancellationToken cancellationToken);

    public Task<IEnumerable<T>> GetAllDocuments(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task<IEnumerable<T>> GetAllDocuments(
        FilterDefinition<T> filter,
        CancellationToken cancellationToken
    );

    public Task<IEnumerable<T>> GetAllDocuments(
        Expression<Func<T, bool>> predicate,
        int offset,
        int limit,
        CancellationToken cancellationToken
    );

    public Task<long> CountDocuments(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task<IEnumerable<T>> GetAllDocuments(
        Expression<Func<T, bool>> predicate,
        SortDefinition<T>? sortDefinition,
        CancellationToken cancellationToken
    );

    public Task UpdateDocument(T model, CancellationToken cancellationToken);

    public Task<T?> DeleteDocument(string id, CancellationToken cancellationToken);

    public Task DeleteDocument(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken
    );
}
