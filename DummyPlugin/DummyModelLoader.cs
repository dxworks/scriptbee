using System.Collections.Generic;
using System.Text.Json;
using ScriptBeePlugin;

namespace DummyPlugin
{
    public class DummyModelLoader : IModelLoader
    {
        public Dictionary<string, Dictionary<string, ScriptBeeModel>> LoadModel(List<string> fileContents, Dictionary<string, object> configuration = null)
        {
            Dictionary<string, Dictionary<string, ScriptBeeModel>> exposedEntities =
                new Dictionary<string, Dictionary<string, ScriptBeeModel>>();

            Dictionary<string, ScriptBeeModel> objectsDictionary = new Dictionary<string, ScriptBeeModel>();

            for (var i = 0; i < fileContents.Count; i++)
            {
                var fileContent = fileContents[i];
                objectsDictionary.Add(i.ToString(), JsonSerializer.Deserialize<DummyModel>(fileContent));
            }

            exposedEntities.Add("DummyModel", objectsDictionary);

            return exposedEntities;
        }

        public string GetName()
        {
            return "Dummy";
        }
    }
}