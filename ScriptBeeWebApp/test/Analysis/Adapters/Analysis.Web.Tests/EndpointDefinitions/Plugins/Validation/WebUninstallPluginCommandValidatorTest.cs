using FluentValidation.TestHelper;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Validation;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Plugins.Validation;

public class WebUninstallPluginCommandValidatorTest
{
    private readonly WebUninstallPluginCommandValidator _validator = new();

    [Fact]
    public async Task GivenValidUninstallPluginCommand_ThenResultHasNoErrors()
    {
        var command = new WebUninstallPluginCommand("test-plugin", "1.0.0");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyPluginId_ThenResultHasErrors()
    {
        var command = new WebUninstallPluginCommand("", "1.0.0");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.PluginId);
    }

    [Fact]
    public async Task GivenEmptyVersion_ThenResultHasErrors()
    {
        var command = new WebUninstallPluginCommand("test-plugin", "");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.Version);
    }

    [Fact]
    public async Task GivenNullPluginId_ThenResultHasErrors()
    {
        var command = new WebUninstallPluginCommand(null!, "1.0.0");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.PluginId);
    }

    [Fact]
    public async Task GivenNullVersion_ThenResultHasErrors()
    {
        var command = new WebUninstallPluginCommand("test-plugin", null!);

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.Version);
    }

    [Fact]
    public async Task GivenEmptyPluginIdAndVersion_ThenResultHasErrors()
    {
        var command = new WebUninstallPluginCommand("", "");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.PluginId);
        result.ShouldHaveValidationErrorFor(x => x.Version);
    }

    [Fact]
    public async Task GivenAllFieldsNull_ThenResultHasErrors()
    {
        var command = new WebUninstallPluginCommand(null!, null!);

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.PluginId);
        result.ShouldHaveValidationErrorFor(x => x.Version);
    }
}
