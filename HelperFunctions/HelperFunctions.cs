using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ScriptBeePlugin;

namespace HelperFunctions
{
    public class HelperFunctions
    {
        public string OutputFolderPath { private get; set; }

        public void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        public void ExportJSON(object obj, string exportPath)
        {
            var serializedObj = JsonConvert.SerializeObject(obj);
            WriteToFile(serializedObj, exportPath);
        }

        public IEnumerable<Dictionary<string, ScriptBeeModel>> Get(
            Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context, string exportedType )
        {
            return context.Where(pair => pair.Key.Item1.Equals(exportedType)).Select(pair => pair.Value).ToList();
        }

        public void WriteToFile(string text, string filePath)
        {
            var outputPath = Path.Combine(OutputFolderPath, filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, text);
        }
    }
}