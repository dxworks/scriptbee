using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
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

        public void ExportJson(string filePath, object obj)
        {
            var outputPath = InitializePath(filePath);

            var jsonSerializer = JsonSerializer.Create();
            
            using (var writer = new StreamWriter(outputPath))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonSerializer.Serialize(jsonWriter, obj);
                }
            }
        }
        
        public object ImportJson(string filePath, object obj)
        {
            var inputPath = InitializePath(filePath);

            var jsonSerializer = JsonSerializer.Create();
            
            using (var reader = new StreamReader(inputPath))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return jsonSerializer.Deserialize(jsonReader, obj.GetType());
                }
            }
        }
        
        public T ImportJson<T>(string filePath)
        {
            var inputPath = InitializePath(filePath);

            var jsonSerializer = JsonSerializer.Create();
            
            using (var reader = new StreamReader(inputPath))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return jsonSerializer.Deserialize<T>(jsonReader);
                }
            }
        }

        public void ExportCsv(string filePath, List<object> records)
        {
            var outputPath = InitializePath(filePath);

            using (var writer = new StreamWriter(outputPath))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                }
            }
        }
        
        public void AppendToCsv(string filePath, List<object> records)
        {
            var outputPath = InitializePath(filePath);
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var stream = File.Open(outputPath, FileMode.Append))
            {
                using (var writer = new StreamWriter(stream))
                {
                    using (var csv = new CsvWriter(writer, config))
                    {
                        csv.WriteRecords(records);
                    }
                }
            }
        }

        public IEnumerable<Dictionary<string, ScriptBeeModel>> Get(
            Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context, string exportedType )
        {
            return context.Where(pair => pair.Key.Item1.Equals(exportedType)).Select(pair => pair.Value).ToList();
        }

        public void WriteToFile(string filePath, string text)
        {
            var outputPath = InitializePath(filePath);

            File.WriteAllText(outputPath, text);
        }
        
        public void AppendToFile(string filePath, string text)
        {
            var outputPath = InitializePath(filePath);

            if (!File.Exists(outputPath))
            {
                using (StreamWriter streamWriter = File.CreateText(outputPath))
                {
                    streamWriter.WriteLine(text);
                }	
            }
            else
            {
                using (StreamWriter streamWriter = File.AppendText(outputPath))
                {
                    streamWriter.WriteLine(text);
                }	
            }
        }

        private string InitializePath(string path)
        {
            var outputPath = Path.Combine(OutputFolderPath, path);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            return outputPath;
        }
    }
}