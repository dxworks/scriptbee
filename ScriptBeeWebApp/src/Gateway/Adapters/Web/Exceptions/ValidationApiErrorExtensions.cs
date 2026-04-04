using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web.Extensions;

namespace ScriptBee.Web.Exceptions;

public static class ValidationApiErrorExtensions
{
    public static BadRequest<ProblemDetails> ToInvalidPaginationProblem(
        HttpContext context,
        int offset,
        int limit
    )
    {
        return TypedResults.BadRequest(
            context.ToProblemDetails(
                "Invalid pagination parameters.",
                $"Offset must be greater than or equal to 0 and limit must be greater than 0. Received offset: {offset}, limit: {limit}."
            )
        );
    }
}
