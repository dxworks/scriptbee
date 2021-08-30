using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeTests.Scripts.ScriptSampleGenerators;
using Xunit;

namespace ScriptBeeTests.Utils.ValidScriptExtractors
{
    public class PythonValidScriptExtractorTest
    {
        private readonly PythonValidScriptExtractor _pythonExtractor;

        private readonly FileContentProvider _fileContentProvider = FileContentProvider.Instance;

        private readonly string _validScript;

        public PythonValidScriptExtractorTest()
        {
            _pythonExtractor = new PythonValidScriptExtractor();
            _validScript =
                _fileContentProvider.GetFileContent(
                    "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonValidScript.txt");
        }

        [Fact]
        public void ExtractValidScript_WithTextBefore()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonScriptWithTextBeforeStartComment.txt");

            var extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_WithTextAfter()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonScriptWithTextAfterEndComment.txt");

            var extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_NoExtraText()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonScriptWithNoExtraText.txt");

            var extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_TextBeforeAndAfter()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonScriptWithTextBeforeAndAfter.txt");

            var extractedScript = _pythonExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
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