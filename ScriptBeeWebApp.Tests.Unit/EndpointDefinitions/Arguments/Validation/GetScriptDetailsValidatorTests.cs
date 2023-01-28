using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class GetScriptDetailsValidatorTests
{
    private readonly GetScriptDetailsValidator _validator;
    
    public GetScriptDetailsValidatorTests()
    {
        _validator = new GetScriptDetailsValidator();
    }
    
    [Fact]
    public async Task GivenValidGetScriptDetails_WhenValidate_ThenResultHasNoErrors()
    {
        var getScriptDetails = new GetScriptDetails("id1", "path");
        
        var result = await _validator.TestValidateAsync(getScriptDetails);
        
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var getScriptDetails = new GetScriptDetails("", "path");
        
        var result = await _validator.TestValidateAsync(getScriptDetails);
        
        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }
    
    [Fact]
    public async Task GivenEmptyFilePath_WhenValidate_ThenResultHasErrors()
    {
        var getScriptDetails = new GetScriptDetails("projectId", "");
        
        var result = await _validator.TestValidateAsync(getScriptDetails);
        
        result.ShouldHaveValidationErrorFor(r => r.FilePath);
    }
}
