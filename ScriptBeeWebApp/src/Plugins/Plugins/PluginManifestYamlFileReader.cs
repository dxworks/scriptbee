using ScriptBee.Domain.Model.Plugins.Manifest;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ScriptBee.Plugins;

public class PluginManifestYamlFileReader(IPluginDiscriminatorHolder pluginDiscriminatorHolder)
    : IPluginManifestYamlFileReader
{
    public PluginManifest Read(string filePath)
    {
        var namingConvention = CamelCaseNamingConvention.Instance;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(namingConvention)
            .WithTypeDiscriminatingNodeDeserializer(o =>
            {
                o.AddKeyValueTypeDiscriminator<PluginExtensionPoint>(
                    namingConvention.Apply(nameof(PluginBundleExtensionPoint.Kind)),
                    pluginDiscriminatorHolder.GetDiscriminatedTypes()
                );
            })
            .Build();

        using var reader = new StreamReader(filePath);

        return deserializer.Deserialize<PluginManifest>(reader);
    }
}
