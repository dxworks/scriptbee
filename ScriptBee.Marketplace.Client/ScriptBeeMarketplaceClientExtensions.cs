using Firebase.Storage;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Marketplace.Client.Data;
using ScriptBee.Marketplace.Client.Repository;
using ScriptBee.Marketplace.Client.Services;

namespace ScriptBee.Marketplace.Client;

public static class ScriptBeeMarketplaceClientExtensions
{
    public static void AddScriptBeeMarketplaceClient(this IServiceCollection services, IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("ScriptBeeMarketplaceClient")
            .Get<ScriptBeeMarketplaceClientConfig>();

        if (configurationSection is null)
        {
            throw new InvalidFirebaseConfigException();
        }

        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configurationSection.FirebaseConfigFile);

        services.AddSingleton<IDocumentRepository<Plugin>, FirebaseDocumentRepository<Plugin>>(_ =>
            new FirebaseDocumentRepository<Plugin>(FirestoreDb.Create(configurationSection.FirestoreDbName)));
        services.AddSingleton<IStorageRepository, FirebaseStorageRepository>(_ =>
            new FirebaseStorageRepository(new FirebaseStorage(configurationSection.FirebaseStorageBucket)));
        services.AddSingleton<IPluginFetcher, PluginFetcher>();
    }
}
