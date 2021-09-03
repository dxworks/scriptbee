using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class FileContentProvider : IFileContentProvider
    {
        public Task<string> GetFileContentAsync(string path)
        {
            return File.ReadAllTextAsync(path);
        }

        public string GetFileContent(string path)
        {
            return File.ReadAllText(path);
        }
    }
}