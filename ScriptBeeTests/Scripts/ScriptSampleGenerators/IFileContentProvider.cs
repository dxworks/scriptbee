namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public interface IFileContentProvider
    {
        public string GetFileContent(string filePath);
    }
}