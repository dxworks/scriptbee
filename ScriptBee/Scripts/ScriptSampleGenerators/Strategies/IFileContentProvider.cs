namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public interface IFileContentProvider
    {
        public string GetFileContent(string path);
    }
}