namespace ScriptBee.Marketplace.Client;

public class ScriptBeeMarketplaceClientConfig
{
    public string FirebaseConfigFile { get; init; } = null!;
    public string FirestoreDbName { get; init; } = null!;
    public string FirebaseStorageBucket { get; init; } = null!;
}
