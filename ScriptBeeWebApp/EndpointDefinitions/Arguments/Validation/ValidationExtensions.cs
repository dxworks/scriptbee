using FluentValidation.Results;
using ScriptBeeWebApp.EndpointDefinitions.DTO;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public static class ValidationExtensions
{
    public static ValidationErrorsResponse GetValidationErrorsResponse(this ValidationResult validationResult)
    {
        return new ValidationErrorsResponse(
            validationResult.Errors.Select(x => new ValidationError(x.PropertyName, x.ErrorMessage))
                .ToList());
    }
}
