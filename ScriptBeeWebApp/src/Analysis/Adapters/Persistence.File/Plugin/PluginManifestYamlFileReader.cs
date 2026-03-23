using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Persistence.File.Plugin.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace ScriptBee.Persistence.File.Plugin;

public class PluginManifestYamlFileReader(IPluginDiscriminatorHolder pluginDiscriminatorHolder)
    : IPluginManifestYamlFileReader
{
    public PluginManifest Read(string filePath)
    {
        var namingConvention = CamelCaseNamingConvention.Instance;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(namingConvention)
            .WithNodeDeserializer(
                inner => new AbstractNodeNodeTypeResolver(
                    inner,
                    new PluginManifestSpecTypeDiscriminator(
                        namingConvention,
                        pluginDiscriminatorHolder.GetDiscriminatedTypes()
                    )
                ),
                s => s.InsteadOf<ObjectNodeDeserializer>()
            )
            .Build();

        using var reader = new StreamReader(filePath);

        return deserializer.Deserialize<PluginManifest>(reader);
    }
}
