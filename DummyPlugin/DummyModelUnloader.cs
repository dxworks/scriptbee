using System.Text.Json;

namespace DummyPlugin
{
    public class DummyModelUnloader
    {
        public string UnloadModel(DummyModel model)
        {
            return JsonSerializer.Serialize(model);
        }
    }
}