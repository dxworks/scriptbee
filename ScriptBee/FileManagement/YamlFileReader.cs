using System.IO;
using YamlDotNet.Serialization;

namespace ScriptBee.FileManagement;

public class YamlFileReader : IYamlFileReader
{
    public T Read<T>(string filePath)
    {
        var deserializer = new DeserializerBuilder()
            .Build();

        using var reader = new StreamReader(filePath);

        return deserializer.Deserialize<T>(reader);
    }
}
