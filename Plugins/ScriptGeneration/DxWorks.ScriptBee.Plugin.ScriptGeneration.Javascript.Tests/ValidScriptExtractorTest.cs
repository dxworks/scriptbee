using System.Threading.Tasks;
using Xunit;

namespace DxWorks.ScriptBee.Plugin.ScriptGeneration.Javascript.Tests;

public class ValidScriptExtractorTest : IAsyncLifetime
{
    private string _validScript = "";

    public async Task InitializeAsync()
    {
        _validScript =
            await RelativeFileContentProvider.GetFileContentAsync(
                "ExtractorsTestStrings/JavascriptValidScript.txt");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ExtractValidScript_WithTextBefore()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/JavascriptScriptWithTextBeforeStartComment.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_WithTextAfter()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/JavascriptScriptWithTextAfterEndComment.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_NoExtraText()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/JavascriptScriptWithNoExtraText.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_TextBeforeAndAfter()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/JavascriptScriptWithTextBeforeAndAfter.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

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
        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal("", extractedScript);
    }
}
