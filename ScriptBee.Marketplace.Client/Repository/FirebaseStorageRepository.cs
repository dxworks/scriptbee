using Firebase.Storage;

namespace ScriptBee.Marketplace.Client.Repository;

public class FirebaseStorageRepository : IStorageRepository
{
    private readonly FirebaseStorage _firebaseStorage;


    public FirebaseStorageRepository(FirebaseStorage firebaseStorage)
    {
        _firebaseStorage = firebaseStorage;
    }

    public async Task<string> GetDownloadUrlAsync(string url)
    {
        return await _firebaseStorage
            .Child(url)
            .GetDownloadUrlAsync();
    }
}
