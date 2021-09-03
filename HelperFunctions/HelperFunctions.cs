﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ScriptBeePlugin;

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

        public static Dictionary<string, ScriptBeeModel> Get(Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> arg)
        {
            return arg.Select(pair => pair.Value).FirstOrDefault();
        }
    }
}