using ScriptBee.Models.Dummy;

namespace ScriptBee.Scripts
{
    public class PythonTemplateGenerator : IDummyTemplateGenerator
    {
        public string GenerateTemplate(DummyModel dummyModel)
        {
            return @"
class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool


model: DummyModel

# start script

print(model.DummyNumber)
print(model.DummyString)
print(model.IsDummy)

# end script
";
        }
    }
}