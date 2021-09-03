using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class RelativeFileContentProvider : IFileContentProvider
    {
        public Task<string> GetFileContentAsync(string path)
        {
            return File.ReadAllTextAsync(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                path));
        }

        public string GetFileContent(string path)
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                path));
        }
    }
}