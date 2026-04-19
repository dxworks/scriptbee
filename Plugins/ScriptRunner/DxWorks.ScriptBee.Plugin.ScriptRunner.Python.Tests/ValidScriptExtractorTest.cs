namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python.Tests;

public class ValidScriptExtractorTest : IAsyncLifetime
{
    private string _validScript = "";

    public async ValueTask InitializeAsync()
    {
        _validScript = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonValidScript.txt"
        );
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task ExtractValidScript_WithTextBefore()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithTextBeforeStartComment.txt",
            TestContext.Current.CancellationToken
        );

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_WithTextAfter()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithTextAfterEndComment.txt",
            TestContext.Current.CancellationToken
        );

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_NoExtraText()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithNoExtraText.txt",
            TestContext.Current.CancellationToken
        );

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_TextBeforeAndAfter()
    {
        var script = await RelativeFileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithTextBeforeAndAfter.txt",
            TestContext.Current.CancellationToken
        );

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("class DummyModel:")]
    [InlineData(
        """
            class DummyModel:

            # start script

            some text
            """
    )]
    [InlineData(
        """
            class DummyModel:

            # end script
            """
    )]
    public void ExtractValidScript_InvalidCases(string? script)
    {
        var extractedScript = ValidScriptExtractor.ExtractValidScript(script!);

        Assert.Equal("", extractedScript);
    }
}
