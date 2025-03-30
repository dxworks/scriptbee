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

    public static NotFound<ProblemDetails> ToProblem(
        this InstanceDoesNotExistsError error,
        HttpContext context
    )
    {
        return TypedResults.NotFound(
            context.ToProblemDetails(
                "Instance Not Found",
                $"An instance with id '{error.InstanceId}' is not allocated."
            )
        );
    }

    public static NotFound<ProblemDetails> ToProblem(
        this AnalysisDoesNotExistsError error,
        HttpContext context
    )
    {
        return TypedResults.NotFound(
            context.ToProblemDetails(
                "Analysis Not Found",
                $"An analysis with the ID '{error.Id}' does not exists."
            )
        );
    }

    public static NotFound<ProblemDetails> ToProblem(
        this AnalysisResultDoesNotExistsError error,
        HttpContext context
    )
    {
        return TypedResults.NotFound(
            context.ToProblemDetails(
                "Result Not Found",
                $"An analysis result with the ID '{error.Id}' does not exists."
            )
        );
    }

    public static Conflict<ProblemDetails> ToProblem(
        this ProjectIdAlreadyInUseError error,
        HttpContext context
    )
    {
        return TypedResults.Conflict(
            context.ToProblemDetails(
                "Project ID Already In Use",
                $"A project with the ID '{error.Id}' already exists. Use a unique Project ID or update the existing project."
            )
        );
    }

    public static Conflict<ProblemDetails> ToProblem(
        this ScriptPathAlreadyExistsError error,
        HttpContext context
    )
    {
        return TypedResults.Conflict(
            context.ToProblemDetails(
                "Script Path Already Exists",
                "A script at that path already exists."
            )
        );
    }
}
