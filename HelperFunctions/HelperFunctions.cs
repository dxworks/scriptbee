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
        public string ProjectId { private get; set; }

        public void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        public void ExportJSON(object obj, string exportPath)
        {
            var serializedObj = JsonConvert.SerializeObject(obj);
            WriteToFile(serializedObj, exportPath);
        }

        public Dictionary<string, ScriptBeeModel> Get(
            Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> arg)
        {
            //todo remove FirstOrDefault to return a Collection of Dictionary<string, ScriptBeeModel>
            //use exportedType as parameter to the function
            var exportedType = "";
            return arg.Where(pair => pair.Key.Item1.Equals(exportedType)).Select(pair => pair.Value).FirstOrDefault();
        }

        public void WriteToFile(string text, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, text);
        }
    }
}