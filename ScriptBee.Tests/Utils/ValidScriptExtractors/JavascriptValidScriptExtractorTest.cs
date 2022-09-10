using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using ScriptBee.Utils.ValidScriptExtractors;
using Xunit;

namespace ScriptBeeTests.Utils.ValidScriptExtractors;

public class JavascriptValidScriptExtractorTest : IAsyncLifetime
{
    private readonly JavascriptValidScriptExtractor _javascriptExtractor;

    private readonly IFileContentProvider _fileContentProvider = new RelativeFileContentProvider();
    private string _validScript = "";

    public JavascriptValidScriptExtractorTest()
    {
        _javascriptExtractor = new JavascriptValidScriptExtractor();
    }

    public async Task InitializeAsync()
    {
        _validScript =
            await _fileContentProvider.GetFileContentAsync(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptValidScript.txt");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ExtractValidScript_WithTextBefore()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptScriptWithTextBeforeStartComment.txt");

        var extractedScript = _javascriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_WithTextAfter()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptScriptWithTextAfterEndComment.txt");

        var extractedScript = _javascriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_NoExtraText()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "Utils/ValidScriptExtractors/ExtractorsTestStrings/JavascriptScriptWithNoExtraText.txt");

        var extractedScript = _javascriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_TextBeforeAndAfter()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
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
        var extractedScript = _javascriptExtractor.ExtractValidScript(script);

        Assert.Equal("", extractedScript);
    }
}
