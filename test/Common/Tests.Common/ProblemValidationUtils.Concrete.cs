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
}
