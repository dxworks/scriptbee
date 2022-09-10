﻿using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using Xunit;

namespace DxWorks.ScriptBee.Plugin.ScriptGeneration.Python.Tests;

public class ValidScriptExtractorTest : IAsyncLifetime
{
    private readonly IFileContentProvider _fileContentProvider = new RelativeFileContentProvider();
    private string _validScript = "";

    public async Task InitializeAsync()
    {
        _validScript =
            await _fileContentProvider.GetFileContentAsync(
                "ExtractorsTestStrings/PythonValidScript.txt");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ExtractValidScript_WithTextBefore()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithTextBeforeStartComment.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_WithTextAfter()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithTextAfterEndComment.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_NoExtraText()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithNoExtraText.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal(_validScript, extractedScript);
    }

    [Fact]
    public async Task ExtractValidScript_TextBeforeAndAfter()
    {
        var script = await _fileContentProvider.GetFileContentAsync(
            "ExtractorsTestStrings/PythonScriptWithTextBeforeAndAfter.txt");

        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

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
        var extractedScript = ValidScriptExtractor.ExtractValidScript(script);

        Assert.Equal("", extractedScript);
    }
}
