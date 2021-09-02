using System.Text.Json;
using ScriptBeePlugin;

namespace DummyPlugin
{
    public class DummyModelLoader : IModelLoader
    {
        public ScriptBeeModel LoadModel(string json)
        {
            return JsonSerializer.Deserialize<DummyModel>(json);
        }
    }
}