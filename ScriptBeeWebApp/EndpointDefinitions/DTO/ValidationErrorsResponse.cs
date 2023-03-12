namespace ScriptBeeWebApp.EndpointDefinitions.DTO;

public record ValidationErrorsResponse(List<ValidationError> Errors) : EndpointError("Validation Error");

public record ValidationError(string PropertyName, string ErrorMessage);
