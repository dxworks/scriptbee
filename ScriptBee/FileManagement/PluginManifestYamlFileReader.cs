using System.IO;
using ScriptBee.FileManagement.Yaml;
using ScriptBee.Plugin.Manifest;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace ScriptBee.FileManagement;

public class PluginManifestYamlFileReader : IPluginManifestYamlFileReader
{
    private readonly IPluginDiscriminatorHolder _pluginDiscriminatorHolder;

    public PluginManifestYamlFileReader(IPluginDiscriminatorHolder pluginDiscriminatorHolder)
    {
        _pluginDiscriminatorHolder = pluginDiscriminatorHolder;
    }

    public PluginManifest Read(string filePath)
    {
        var namingConvention = CamelCaseNamingConvention.Instance;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(namingConvention)
            .WithNodeDeserializer(
                inner => new AbstractNodeNodeTypeResolver(inner,
                    new PluginManifestSpecTypeDiscriminator(namingConvention,
                        _pluginDiscriminatorHolder.GetDiscriminatedTypes())),
                s => s.InsteadOf<ObjectNodeDeserializer>())
            .Build();

        using var reader = new StreamReader(filePath);

        return deserializer.Deserialize<PluginManifest>(reader);
    }
}
