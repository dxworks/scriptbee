using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class DownloadFileValidatorTests
{
    private readonly DownloadFileValidator _validator;

    public DownloadFileValidatorTests()
    {
        _validator = new DownloadFileValidator();
    }

    [Fact]
    public async Task GivenValidDownloadFile_WhenValidate_ThenResultHasNoErrors()
    {
        var downloadFile = new DownloadFile(Guid.NewGuid(), "name");

        var result = await _validator.TestValidateAsync(downloadFile);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyId_WhenValidate_ThenResultHasErrors()
    {
        var downloadFile = new DownloadFile(Guid.Empty, "name");

        var result = await _validator.TestValidateAsync(downloadFile);

        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenEmptyName_WhenValidate_ThenResultHasErrors()
    {
        var downloadFile = new DownloadFile(Guid.NewGuid(), "");

        var result = await _validator.TestValidateAsync(downloadFile);

        result.ShouldHaveValidationErrorFor(r => r.Name);
    }
}
