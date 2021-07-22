using ScriptBee.Models;

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


# start script

model: DummyModel
print(model)

# end script
";
        }
    }
}