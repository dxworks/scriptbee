using FluentValidation.Results;

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

public record ValidationErrorsResponse(List<ValidationError> Errors);

public record ValidationError(string PropertyName, string ErrorMessage);
