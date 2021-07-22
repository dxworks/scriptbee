using System.IO;

namespace ScriptBee.Scripts
{
    public class ScriptLoader : IScriptLoader
    {
        public string LoadScript(string pathToScript)
        {
            return File.ReadAllText(pathToScript);
        }
    }
}