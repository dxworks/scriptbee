using System.Threading.Tasks;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public interface IFileContentProvider
    {
        public Task<string> GetFileContentAsync(string path);
        public string GetFileContent(string path);
    }
}