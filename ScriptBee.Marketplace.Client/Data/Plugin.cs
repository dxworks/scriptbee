using Google.Cloud.Firestore;

namespace ScriptBee.Marketplace.Client.Data;

[FirestoreData]
public class Plugin
{
    [FirestoreProperty("name")] public string Name { get; init; }
    [FirestoreProperty("author")] public string Author { get; init; }
    [FirestoreProperty("description")] public string Description { get; init; }
    [FirestoreProperty("versions")] public List<PluginVersion> Versions { get; init; }

    public void Deconstruct(out string name, out string author, out string description,
        out List<PluginVersion> versions)
    {
        name = Name;
        author = Author;
        description = Description;
        versions = Versions;
    }
}

[FirestoreData]
public class PluginVersion
{
    [FirestoreProperty("url")] public string Url { get; init; }
    [FirestoreProperty("version")] public string Version { get; init; }
    [FirestoreProperty("extensionPoints")] public List<ExtensionPointVersion> ExtensionPoints { get; init; }

    public void Deconstruct(out string url, out string version, out List<ExtensionPointVersion> extensionPoints)
    {
        url = Url;
        version = Version;
        extensionPoints = ExtensionPoints;
    }
}

[FirestoreData]
public class ExtensionPointVersion
{
    [FirestoreProperty("kind")] public string Kind { get; init; }
    [FirestoreProperty("version")] public string Version { get; init; }

    public void Deconstruct(out string kind, out string version)
    {
        kind = Kind;
        version = Version;
    }
}
