using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.Web.Exceptions;

public static class ApiErrorExtensions
{
    public static NotFound<ProblemDetails> ToProblem(
        this ProjectDoesNotExistsError error,
        HttpContext context
    )
    {
        return TypedResults.NotFound(
            context.ToProblemDetails(
                "Project Not Found",
                $"A project with the ID '{error.Id}' does not exists."
            )
        );
    }
}
