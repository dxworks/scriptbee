using System.IO;
using System.Reflection;

namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class FileContentProvider : IFileContentProvider
    {
        public string GetFileContent(string filePath)
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                filePath));
        }
    }
}