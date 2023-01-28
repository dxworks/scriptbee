using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class RunScriptValidatorTests
{
    private readonly RunScriptValidator _runScriptValidator;

    public RunScriptValidatorTests()
    {
        _runScriptValidator = new RunScriptValidator();
    }

    [Fact]
    public async Task GivenValidRunScript_WhenValidate_ThenResultHasNoErrors()
    {
        var runScript = new RunScript("id", "path", "language");

        var result = await _runScriptValidator.TestValidateAsync(runScript);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var runScript = new RunScript("", "path", "language");

        var result = await _runScriptValidator.TestValidateAsync(runScript);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GivenEmptyFilePath_WhenValidate_ThenResultHasErrors()
    {
        var runScript = new RunScript("id", "", "language");

        var result = await _runScriptValidator.TestValidateAsync(runScript);

        result.ShouldHaveValidationErrorFor(x => x.FilePath);
    }
    
    [Fact]
    public async Task GivenEmptyLanguage_WhenValidate_ThenResultHasErrors()
    {
        var runScript = new RunScript("id", "path", "");

        var result = await _runScriptValidator.TestValidateAsync(runScript);

        result.ShouldHaveValidationErrorFor(x => x.Language);
    }   
}
