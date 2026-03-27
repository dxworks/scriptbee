using FluentValidation.TestHelper;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Validation;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Plugins.Validation;

public class WebInstallPluginCommandValidatorTest
{
    private readonly WebInstallPluginCommandValidator _validator = new();

    [Fact]
    public async Task GivenValidInstallPluginCommand_ThenResultHasNoErrors()
    {
        var command = new WebInstallPluginCommand("test-plugin", "1.0.0");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyPluginId_ThenResultHasErrors()
    {
        var command = new WebInstallPluginCommand("", "1.0.0");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.PluginId);
    }

    [Fact]
    public async Task GivenEmptyVersion_ThenResultHasErrors()
    {
        var command = new WebInstallPluginCommand("test-plugin", "");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.Version);
    }

    [Fact]
    public async Task GivenNullPluginId_ThenResultHasErrors()
    {
        var command = new WebInstallPluginCommand(null!, "1.0.0");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.PluginId);
    }

    [Fact]
    public async Task GivenNullVersion_ThenResultHasErrors()
    {
        var command = new WebInstallPluginCommand("test-plugin", null!);

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.Version);
    }

    [Fact]
    public async Task GivenEmptyPluginIdAndVersion_ThenResultHasErrors()
    {
        var command = new WebInstallPluginCommand("", "");

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
        var command = new WebInstallPluginCommand(null!, null!);

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldHaveValidationErrorFor(x => x.PluginId);
        result.ShouldHaveValidationErrorFor(x => x.Version);
    }
}
