using System.IO;
using System.Reflection;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class FileContentProvider : IFileContentProvider
    {
        private FileContentProvider()
        {
        }

        public static FileContentProvider Instance { get; } = new FileContentProvider();

        public string GetFileContent(string path)
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                path));
        }
    }
}