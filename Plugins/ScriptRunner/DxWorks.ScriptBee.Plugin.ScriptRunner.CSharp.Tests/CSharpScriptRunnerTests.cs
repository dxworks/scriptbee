using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
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
        var scriptContent = @"
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
}".Replace("\r\n", Environment.NewLine);

        await _scriptRunner.RunAsync(project, helperFunctionsContainer, scriptContent, It.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GivenEmptyFileWithDummyHelperFunction_WhenRunAsync_FileIsCompiledWithoutErrors()
    {
        var project = new Mock<IProject>().Object;
        var helperFunctionsContainer = new HelperFunctionsContainer(new List<IHelperFunctions>
        {
            new DummyHelperFunctions()
        });

        var scriptContent = @"
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
}".Replace("\r\n", Environment.NewLine);

        await _scriptRunner.RunAsync(project, helperFunctionsContainer, scriptContent, It.IsAny<CancellationToken>());
    }
}
