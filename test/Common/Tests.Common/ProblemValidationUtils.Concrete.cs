namespace ScriptBee.Tests.Common;

public static partial class ProblemValidationUtils
{
    public static async Task AssertProjectNotFoundProblem(
        HttpResponseMessage response,
        string testUrl,
        string projectId = "project-id"
    )
    {
        await AssertNotFoundProblem(
            response.Content,
            testUrl,
            "Project Not Found",
            $"A project with the ID '{projectId}' does not exists."
        );
    }
}
