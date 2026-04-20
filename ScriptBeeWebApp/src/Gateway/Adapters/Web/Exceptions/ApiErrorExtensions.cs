using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

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

    public static ValidationProblem ToProblem(
        this ScriptLanguageDoesNotExistsError error,
        HttpContext context
    )
    {
        return TypedResults.ValidationProblem(
            new Dictionary<string, string[]>
            {
                {
                    nameof(WebCreateScriptCommand.Language),
                    [$"'{error.Language}' language does not exists."]
                },
            }
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

    public static NotFound<ProblemDetails> ToProblem(
        this PluginNotFoundError error,
        HttpContext context
    )
    {
        return TypedResults.NotFound(
            context.ToProblemDetails(
                "Plugin Not Found",
                $"A plugin with the ID '{error.Id.Name}' does not exists."
            )
        );
    }

    public static InternalServerError<ProblemDetails> ToProblem(
        this PluginInstallationError error,
        HttpContext context
    )
    {
        var additionalMessage = error
            .NestedPluginsThatCouldNotBeInstalled.Select(id =>
                $"Nested plugin {id.Name} version '{id.Version}' could not be installed."
            )
            .Aggregate(" ", (s, s1) => s + s1)
            .TrimEnd();

        return TypedResults.InternalServerError(
            context.ToProblemDetails(
                "Plugin Installation Failed",
                $"Could not install plugin {error.Id.Name} with version '{error.Id.Version}'.{additionalMessage}"
            )
        );
    }

    public static NotFound<ProblemDetails> ToProblem(
        this ScriptDoesNotExistsError error,
        HttpContext context
    )
    {
        return TypedResults.NotFound(
            context.ToProblemDetails(
                "Script Not Found",
                $"A script with the ID '{error.ScriptId}' does not exists."
            )
        );
    }

    public static BadRequest<ProblemDetails> ToProblem(
        this PluginManifestNotFoundError error,
        HttpContext context
    )
    {
        return TypedResults.BadRequest(
            context.ToProblemDetails(
                "Plugin Manifest Not Found",
                "The 'manifest.yaml' file was not found at the root of the plugin."
            )
        );
    }

    public static Conflict<ProblemDetails> ToProblem(
        this PluginAlreadyExistsError error,
        HttpContext context
    )
    {
        return TypedResults.Conflict(
            context.ToProblemDetails(
                "Plugin Already Exists",
                $"A plugin with the ID '{error.Id.Name}' and version '{error.Id.Version}' already exists."
            )
        );
    }
}
