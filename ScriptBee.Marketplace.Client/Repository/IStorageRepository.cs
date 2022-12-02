namespace ScriptBee.Marketplace.Client.Repository;

public interface IStorageRepository
{
    Task<string> GetDownloadUrlAsync(string url);
}
