using System.Text.Json;
using Newtonsoft.Json;
using ScriptBeePlugin;

namespace DummyPlugin
{
    public class DummyModelUnloader : IModelUnloader
    {
        public string UnloadModel(ScriptBeeModel model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}