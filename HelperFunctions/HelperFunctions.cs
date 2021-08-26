using System;
using System.IO;
using System.Text.Json;

namespace HelperFunctions
{
    public static class HelperFunctions
    {
        public static void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        public static void ExportJSON(object obj, string exportPath)
        {
            string serializedObj = JsonSerializer.Serialize(obj);
            WriteToFile(serializedObj, exportPath);
        }

        public static void WriteToFile(string text, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, text);
        }
    }
}