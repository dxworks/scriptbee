using System.Text.Json;

namespace ScriptBee.Models.Dummy
{
    public class DummyModelLoader
    {
        public DummyModel LoadModel(string json)
        {
            return JsonSerializer.Deserialize<DummyModel>(json);
        }
    }
}