using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptBee.Utils.ValidScriptExtractors;
using Xunit;

namespace ScriptBeeTests.Utils.ValidScriptExtractors
{
    public class JavascriptValidScriptExtractorTest
    {
        private readonly JavascriptValidScriptExtractor _javascriptExtractor;

        private readonly FileContentProvider _fileContentProvider = FileContentProvider.Instance;

        private readonly string _validScript;

        public JavascriptValidScriptExtractorTest()
        {
            _javascriptExtractor = new JavascriptValidScriptExtractor();
            _validScript =
                _fileContentProvider.GetFileContent(
                    "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptValidScript.txt");
        }
        
        [Fact]
        public void ExtractValidScript_WithTextBefore()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptScriptWithTextBeforeStartComment.txt");

            var extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_WithTextAfter()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptScriptWithTextAfterEndComment.txt");

            var extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_NoExtraText()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptScriptWithNoExtraText.txt");

            var extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
        }

        [Fact]
        public void ExtractValidScript_TextBeforeAndAfter()
        {
            string script = _fileContentProvider.GetFileContent(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptScriptWithTextBeforeAndAfter.txt");

            var extractedScript = _javascriptExtractor.ExtractValidScript(script);
            Assert.Equal(_validScript, extractedScript);
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