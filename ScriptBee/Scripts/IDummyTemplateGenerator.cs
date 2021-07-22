using ScriptBee.Models;

namespace ScriptBee.Scripts
{
    public interface IDummyTemplateGenerator : TemplateGenerator
    {
        public string GenerateTemplate(DummyModel dummyModel);
    }
}