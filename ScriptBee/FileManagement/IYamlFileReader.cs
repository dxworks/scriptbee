namespace ScriptBee.FileManagement;

public interface IYamlFileReader
{
    T Read<T>(string filePath);
}
