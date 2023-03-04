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
}".Replace(Environment.NewLine, "\r\n");

        await _scriptRunner.RunAsync(project, helperFunctionsContainer, new List<ScriptParameter>(), scriptContent,
            It.IsAny<CancellationToken>());
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
}".Replace(Environment.NewLine, "\r\n");

        await _scriptRunner.RunAsync(project, helperFunctionsContainer, new List<ScriptParameter>(), scriptContent,
            It.IsAny<CancellationToken>());
    }
    
    
    [Fact]
    public async Task GivenEmptyFileWithDummyHelperFunctionWithScriptParameters_WhenRunAsync_FileIsCompiledWithoutErrors()
    {
        var project = new Mock<IProject>().Object;
        var helperFunctionsContainer = new HelperFunctionsContainer(new List<IHelperFunctions>
        {
            new DummyHelperFunctions()
        });
        var scriptParameters = new List<ScriptParameter>
        {
            new()
            {
                Name = "a",
                Type = "string",
                Value = "a"
            }
        };
        
        var scriptContent = @"
using System;
using System.Text;
using System.Linq;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;

public class ScriptContent
{
    public void ExecuteScript(IProject project, ScriptParameters scriptParameters)
    {        
    }
}".Replace(Environment.NewLine, "\r\n");

        await _scriptRunner.RunAsync(project, helperFunctionsContainer, scriptParameters, scriptContent,
            It.IsAny<CancellationToken>());
    }
}
