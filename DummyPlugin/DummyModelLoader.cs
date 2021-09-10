using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ScriptBeePlugin;

namespace DummyPlugin
{
    public class DummyModelLoader : IModelLoader
    {
        public Dictionary<string, Dictionary<string, ScriptBeeModel>> LoadModel(List<Stream> fileStreams, Dictionary<string, object> configuration = null)
        {
            Dictionary<string, Dictionary<string, ScriptBeeModel>> exposedEntities =
                new Dictionary<string, Dictionary<string, ScriptBeeModel>>();

            Dictionary<string, ScriptBeeModel> objectsDictionary = new Dictionary<string, ScriptBeeModel>();

            var jsonSerializer = new JsonSerializer();

            for (var i = 0; i < fileStreams.Count; i++)
            {
                using (var streamReader = new StreamReader(fileStreams[i]))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var dummyObject = jsonSerializer.Deserialize<DummyModel>(jsonTextReader);
                        objectsDictionary.Add(i.ToString(), dummyObject);
                    }
                }
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