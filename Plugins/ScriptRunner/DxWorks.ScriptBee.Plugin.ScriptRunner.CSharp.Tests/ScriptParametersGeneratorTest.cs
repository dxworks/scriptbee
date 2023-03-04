using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Exceptions;
using Xunit;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

public class ScriptParametersGeneratorTest
{
    [Fact]
    public void GivenEmptyParameters_WhenGenerateScriptParameters_ThenEmptyClassIsGenerated()
    {
        var scriptParametersCode = GetScriptParametersCode(new List<ScriptParameter>());

        Assert.Equal(@"public class ScriptParameters
{
}", scriptParametersCode);
    }

    [Fact]
    public void GivenParameters_WhenGenerateScriptParameters_ThenClassWithPropertiesIsGenerated()
    {
        var parameters = new List<ScriptParameter>
        {
            new()
            {
                Name = "a",
                Type = "string",
                Value = "value"
            },
            new()
            {
                Name = "b",
                Type = "integer",
                Value = "5"
            },
            new()
            {
                Name = "c",
                Type = "boolean",
                Value = "true"
            },
            new()
            {
                Name = "d",
                Type = "float",
                Value = "6.5"
            }
        };
        var scriptParametersCode = GetScriptParametersCode(parameters);

        Assert.Equal(@"public class ScriptParameters
{
    public string a { get; set; } = ""value"";
    public integer b { get; set; } = 5;
    public boolean c { get; set; } = true;
    public float d { get; set; } = 6.5F;
}", scriptParametersCode);
    }

    [Fact]
    public void GivenParametersWithNullValue_WhenGenerateScriptParameters_ThenClassWithPropertiesIsGenerated()
    {
        var parameters = new List<ScriptParameter>
        {
            new()
            {
                Name = "a",
                Type = "string",
                Value = null
            },
            new()
            {
                Name = "b",
                Type = "integer",
                Value = null
            },
            new()
            {
                Name = "c",
                Type = "boolean",
                Value = null
            },
            new()
            {
                Name = "d",
                Type = "float",
                Value = null
            }
        };
        var scriptParametersCode = GetScriptParametersCode(parameters);

        Assert.Equal(@"public class ScriptParameters
{
    public string a { get; set; }

    public integer b { get; set; }

    public boolean c { get; set; }

    public float d { get; set; }
}", scriptParametersCode);
    }
    
    [Fact]
    public void GivenInvalidParameterType_WhenGenerateScriptParameters_ThenExceptionIsThrown()
    {
        var parameters = new List<ScriptParameter>
        {
            new()
            {
                Name = "a",
                Type = "invalid",
                Value = "123"
            }
        };  

        Assert.Throws<InvalidParameterTypeException>(() => GetScriptParametersCode(parameters));
    }
    
    private static string GetScriptParametersCode(IEnumerable<ScriptParameter> parameters)
    {
        return ScriptParametersGenerator.GenerateScriptParameters(parameters)
            .ToFullString();
    }
}
