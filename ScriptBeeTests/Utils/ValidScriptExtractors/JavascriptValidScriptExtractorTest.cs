using ScriptBee.Utils.ValidScriptExtractors;
using Xunit;

namespace ScriptBeeTests.Utils.ValidScriptExtractors
{
    public class JavascriptValidScriptExtractorTest
    {
        private readonly JavascriptValidScriptExtractor _javascriptExtractor;

        public JavascriptValidScriptExtractorTest()
        {
            _javascriptExtractor = new JavascriptValidScriptExtractor();
        }

        [Fact]
        public void ExtractValidScript_WithTextBefore()
        {
            const string script = @"
class DummyModel {
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}

let model = new DummyModel();

// start script

print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);

// end script 
";
            const string validScript = @"print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);";

            var extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_WithTextAfter()
        {
            const string script = @"// start script

print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);

// end script 

class DummyModel {
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}

let model = new DummyModel();
";
            const string validScript = @"print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);";
            string extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_NoExtraText()
        {
            const string script = @"
// start script

print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);

// end script
";
            const string validScript = @"print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);";
            string extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_TextBeforeAndAfter()
        {
            const string script = @"class DummyModel {
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}

let model = new DummyModel();// start script

print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);

// end script 

class DummyModel {
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}
";
            const string validScript = @"print(model.DummyNumber);
print(model.DummyString);
print(model.IsDummy);";

            string extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(validScript, extractedScript);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("class DummyModel {")]
        [InlineData(@"class DummyModel {

// start script

some text }")]
        [InlineData(@"class DummyModel {

// end script")]
        public void ExtractValidScript_InvalidCases(string script)
        {
            const string invalidScript = "";

            string extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(invalidScript, extractedScript);
        }
    }
}