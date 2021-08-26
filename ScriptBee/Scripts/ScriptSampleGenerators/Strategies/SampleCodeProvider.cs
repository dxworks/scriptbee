using System.IO;
using System.Reflection;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class SampleCodeProvider : ISampleCodeProvider
    {
        private SampleCodeProvider()
        {
        }

        public static SampleCodeProvider Instance { get; } = new SampleCodeProvider();

        public string GetSampleCode(string path)
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                path));
        }
    }
}