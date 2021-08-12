using ScriptBee.Models.Dummy;
using ScriptBee.Scripts.TemplateGenerators;
using ScriptBee.Scripts.TemplateGenerators.Strategies;
using Xunit;

namespace ScriptBeeTests.Scripts.TemplateGenerators
{
    public class TemplateGeneratorTest
    {
        [Fact]
        public void Generate_WithPythonStrategy()
        {
            const string expectedScript = @"class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool

model: DummyModel

# start script

print(model)

# end script
";
            string generatedScript =
                new TemplateGenerator(new PythonStrategyTemplateGenerator()).Generate(typeof(DummyModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy()
        {
            const string expectedScript = @"class DummyModel
{
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}

let model = new DummyModel();

// start script

print(model);

// end script
";
            string generatedScript =
                new TemplateGenerator(new JavascriptStrategyTemplateGenerator()).Generate(typeof(DummyModel));

            Assert.Equal(expectedScript, generatedScript);
        }
    }
}