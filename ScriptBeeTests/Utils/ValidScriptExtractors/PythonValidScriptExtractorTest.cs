using ScriptBee.Utils.ValidScriptExtractors;
using Xunit;

namespace ScriptBeeTests.Utils.ValidScriptExtractors
{
    public class PythonValidScriptExtractorTest
    {
        private readonly PythonValidScriptExtractor _pythonExtractor;

        public PythonValidScriptExtractorTest()
        {
            _pythonExtractor = new PythonValidScriptExtractor();
        }

        [Fact]
        public void ExtractValidScript_WithTextBefore()
        {
            const string script = @"
class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool


# start script

model: DummyModel
print(model)

# end script
";
            const string validScript = @"model: DummyModel
print(model)";
            var extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_WithTextAfter()
        {
            const string script = @"
# start script

model: DummyModel
print(model)

# end script

class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool
";
            string validScript = @"model: DummyModel
print(model)";
            string extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_NoExtraText()
        {
            const string script = @"
# start script

model: DummyModel
print(model)

# end script
";
            const string validScript = @"model: DummyModel
print(model)";
            string extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_TextBeforeAndAfter()
        {
            const string script = @"class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool

# start script

model: DummyModel
print(model)

# end script

class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool
";

            const string validScript = @"model: DummyModel
print(model)";

            string extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("class DummyModel:")]
        [InlineData(@"class DummyModel:

# start script

some text")]
        [InlineData(@"class DummyModel:

# end script")]
        public void ExtractValidScript_InvalidCases(string script)
        {
            const string invalidScript = "";

            string extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(invalidScript, extractedScript);
        }
    }
}