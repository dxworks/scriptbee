namespace ScriptBee.Services;

public interface IResultCollector
{
    void Add(string outputFileName, string type);
}
