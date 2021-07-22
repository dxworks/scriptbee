using System.Text.Json;

namespace ScriptBee.Models
{
    public class ModelUnloader
    {
        public string Unload(DummyModel model)
        {
            return JsonSerializer.Serialize(model);
        }
    }
}