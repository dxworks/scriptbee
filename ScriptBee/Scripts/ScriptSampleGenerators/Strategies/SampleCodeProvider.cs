using System.IO;
using System.Reflection;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class SampleCodeProvider : ISampleCodeProvider
    {
        public string GetSampleCode(string path)
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                path));
        }
    }
}