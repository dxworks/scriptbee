using System.Text.Json;

namespace DummyPlugin
{
    public class DummyModelLoader
    {
        public DummyModel LoadModel(string json)
        {
            return JsonSerializer.Deserialize<DummyModel>(json);
        }
    }
}