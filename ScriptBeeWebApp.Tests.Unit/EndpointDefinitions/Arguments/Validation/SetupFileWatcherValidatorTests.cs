using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class SetupFileWatcherValidatorTests
{
    private readonly SetupFileWatcherValidator _validator;

    public SetupFileWatcherValidatorTests()
    {
        _validator = new SetupFileWatcherValidator();
    }

    [Fact]
    public async Task GivenValidSetupFileWatcher_WhenValidate_ThenResultHasNoErrors()
    {
        var setupFileWatcher = new SetupFileWatcher("id1");

        var result = await _validator.TestValidateAsync(setupFileWatcher);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var setupFileWatcher = new SetupFileWatcher("");

        var result = await _validator.TestValidateAsync(setupFileWatcher);

        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }
}
