using System.Net;

namespace ScriptBee.Tests.Common;

public static partial class ProblemValidationUtils
{
    public static async Task AssertProjectNotFoundProblem(
        HttpResponseMessage response,
        string testUrl,
        string projectId = "project-id"
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            testUrl,
            "Project Not Found",
            $"A project with the ID '{projectId}' does not exists."
        );
    }

    public static async Task AssertInstanceNotFoundProblem(
        HttpResponseMessage response,
        string testUrl,
        string instanceId
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            testUrl,
            "Instance Not Found",
            $"An instance with id '{instanceId}' is not allocated."
        );
    }

    public static async Task AssertAnalysisNotFoundProblem(
        HttpResponseMessage response,
        string testUrl,
        string analysisId
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            testUrl,
            "Analysis Not Found",
            $"An analysis with the ID '{analysisId}' does not exists."
        );
    }

    public static async Task AssertAnalysisResultNotFoundProblem(
        HttpResponseMessage response,
        string testUrl,
        string resultId
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            testUrl,
            "Result Not Found",
            $"An analysis result with the ID '{resultId}' does not exists."
        );
    }

    public static async Task AssertProjectIdExistsConflictProblem(
        HttpResponseMessage response,
        string testUrl,
        string projectId
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await AssertConflictProblem(
            response.Content,
            testUrl,
            "Project ID Already In Use",
            $"A project with the ID '{projectId}' already exists. Use a unique Project ID or update the existing project."
        );
    }

    public static async Task AssertLanguageDoesNotExistsBadRequestProblem(
        HttpResponseMessage response,
        string testUrl,
        string language
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            testUrl,
            new { Language = new List<string> { $"'{language}' language does not exists." } }
        );
    }

    public static async Task AssertScriptPathExistsConflictProblem(
        HttpResponseMessage response,
        string testUrl
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await AssertConflictProblem(
            response.Content,
            testUrl,
            "Script Path Already Exists",
            "A script at that path already exists."
        );
    }

    public static async Task AssertNoInstanceAllocatedForProjectBadRequestProblem(
        HttpResponseMessage response,
        string testUrl,
        string projectId
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertBadRequestProblem(
            response.Content,
            testUrl,
            "No Instance Allocated For Project",
            $"There is no instance allocated for project with the ID '{projectId}'"
        );
    }

    public static async Task AssertNoInstanceAllocatedForProjectNotFoundProblem(
        HttpResponseMessage response,
        string testUrl,
        string projectId = "project-id"
    )
    {
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            testUrl,
            "No Instance Allocated For Project",
            $"There is no instance allocated for project with the ID '{projectId}'"
        );
    }
}
