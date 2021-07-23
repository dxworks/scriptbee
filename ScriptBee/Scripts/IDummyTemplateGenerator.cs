using ScriptBee.Models.Dummy;

namespace ScriptBee.Scripts
{
    public interface IDummyTemplateGenerator : ITemplateGenerator
    {
        public string GenerateTemplate(DummyModel dummyModel);
    }
}