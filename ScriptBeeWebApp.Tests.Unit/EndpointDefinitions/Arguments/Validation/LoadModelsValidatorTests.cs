using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class LoadModelsValidatorTests
{
    private readonly LoadModelsValidator _loadModelsValidator;

    public LoadModelsValidatorTests()
    {
        _loadModelsValidator = new LoadModelsValidator();
    }

    [Fact]
    public async Task GivenValidLoadModels_WhenValidate_ThenResultHasNoErrors()
    {
        var loadModels = new LoadModels("projectId", new List<Node>
        {
            new("loader", new List<string>())
        });

        var result = await _loadModelsValidator.TestValidateAsync(loadModels);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var loadModels = new LoadModels("", new List<Node>
        {
            new("loader", new List<string>())
        });

        var result = await _loadModelsValidator.TestValidateAsync(loadModels);

        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }

    [Fact]
    public async Task GivenEmptyNodes_WhenValidate_ThenResultHasErrors()
    {
        var loadModels = new LoadModels("projectId", new List<Node>());

        var result = await _loadModelsValidator.TestValidateAsync(loadModels);

        result.ShouldHaveValidationErrorFor(r => r.Nodes);
    }
}
