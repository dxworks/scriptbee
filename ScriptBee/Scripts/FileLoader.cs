using System.IO;

namespace ScriptBee.Scripts
{
    public class FileLoader : IFileLoader
    {
        public string LoadFileContent(string pathToScript)
        {
            return File.ReadAllText(pathToScript);
        }
    }
}