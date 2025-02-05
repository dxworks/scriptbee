using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ScriptBee.Common.Web.Validation;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}
