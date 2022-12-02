using Google.Cloud.Firestore;

namespace ScriptBee.Marketplace.Client.Repository;

public class FirebaseDocumentRepository<T> : IDocumentRepository<T>
{
    private readonly FirestoreDb _firestore;

    public FirebaseDocumentRepository(FirestoreDb firestore)
    {
        _firestore = firestore;
    }

    public async Task<T> GetDocumentAsync(string collection, string id, CancellationToken cancellationToken = default)
    {
        var documentReference = _firestore.Collection(collection).Document(id);

        var snapshot = await documentReference.GetSnapshotAsync(cancellationToken);

        return snapshot.ConvertTo<T>();
    }

    public async Task<IEnumerable<T>> GetAllDocumentsAsync(string collection,
        CancellationToken cancellationToken = default)
    {
        var query = _firestore.Collection(collection);
        var allDocumentsAsync = await query.GetSnapshotAsync(cancellationToken);

        return allDocumentsAsync.Documents.Select(document => document.ConvertTo<T>());
    }
}
