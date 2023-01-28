using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class DownloadAllValidatorTests
{
    private readonly DownloadAllValidator _validator;

    public DownloadAllValidatorTests()
    {
        _validator = new DownloadAllValidator();
    }

    [Fact]
    public async Task GivenValidDownloadAllWith0Index_WhenValidate_ThenResultHasNoErrors()
    {
        var downloadAll = new DownloadAll("projectId", 0);

        var result = await _validator.TestValidateAsync(downloadAll);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenValidDownloadAllWithPositiveIndex_WhenValidate_ThenResultHasNoErrors()
    {
        var downloadAll = new DownloadAll("projectId", 1);

        var result = await _validator.TestValidateAsync(downloadAll);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var downloadAll = new DownloadAll("", 0);

        var result = await _validator.TestValidateAsync(downloadAll);

        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }

    [Fact]
    public async Task GivenNegativeRunIndex_WhenValidate_ThenResultHasErrors()
    {
        var downloadAll = new DownloadAll("projectId", -1);

        var result = await _validator.TestValidateAsync(downloadAll);

        result.ShouldHaveValidationErrorFor(r => r.RunIndex);
    }
}
