using System.Text.Json;

namespace ScriptBee.Models.Dummy
{
    public class DummyModelUnloader
    {
        public string UnloadModel(DummyModel model)
        {
            return JsonSerializer.Serialize(model);
        }
    }
}