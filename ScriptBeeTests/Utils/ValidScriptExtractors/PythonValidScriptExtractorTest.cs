using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using ScriptBee.Utils.ValidScriptExtractors;
using Xunit;

namespace ScriptBeeTests.Utils.ValidScriptExtractors;

public class PythonValidScriptExtractorTest : IAsyncLifetime
{
    private readonly PythonValidScriptExtractor _pythonExtractor;
    private readonly IFileContentProvider _fileContentProvider = new RelativeFileContentProvider();
    private string _validScript = "";

    public PythonValidScriptExtractorTest()
    {
        _pythonExtractor = new PythonValidScriptExtractor();
    }

    public async Task InitializeAsync()
    {
        _validScript =
            await _fileContentProvider.GetFileContentAsync(
                "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonValidScript.txt");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ExtractValidScript_WithTextBefore()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonScriptWithTextBeforeStartComment.txt");

        var extractedScript = _pythonExtractor.ExtractValidScript(script);
        
        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_WithTextAfter()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonScriptWithTextAfterEndComment.txt");

        var extractedScript = _pythonExtractor.ExtractValidScript(script);
        
        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_NoExtraText()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "Utils/ValidScriptExtractors/ExtractorsTestStrings/PythonScriptWithNoExtraText.txt");

        var extractedScript = _pythonExtractor.ExtractValidScript(script);
        
        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_TextBeforeAndAfter()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
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
        var extractedScript = _pythonExtractor.ExtractValidScript(script);

        Assert.Equal("", extractedScript);
    }
}
