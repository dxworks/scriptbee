using System.Text.Json;

namespace ScriptBee.Models
{
    public class ModelLoader
    {
        public DummyModel Load(string json)
        {
            return JsonSerializer.Deserialize<DummyModel>(json);
        }
    }
}