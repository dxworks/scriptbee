using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Moq;
using Xunit;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

public class CSharpScriptRunnerTests
{
    private readonly CSharpScriptRunner _scriptRunner;

    public CSharpScriptRunnerTests()
    {
        _scriptRunner = new CSharpScriptRunner();
    }

    [Fact]
    public async Task GivenEmptyFile_WhenRunAsync_FileIsCompiledWithoutErrors()
    {
        var project = new Mock<IProject>().Object;
        var helperFunctionsContainer = new Mock<IHelperFunctionsContainer>().Object;
        const string scriptContent = @"
using System;
using System.Text;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;

public class ScriptContent
{
    public void ExecuteScript(IProject project)
    {        
    }
}";

        await _scriptRunner.RunAsync(project, helperFunctionsContainer, scriptContent, It.IsAny<CancellationToken>());
    }
}
