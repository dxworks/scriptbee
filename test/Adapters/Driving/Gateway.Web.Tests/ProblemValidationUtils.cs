using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ScriptBee.Gateway.Web.Tests;

public static class ProblemValidationUtils
{
    public static async Task AssertEmptyRequestBodyProblem(HttpContent responseContent, string url)
    {
        var problemDetails = (await responseContent.ReadFromJsonAsync<ProblemDetails>())!;

        AssertBadRequest(url, problemDetails, "Request body is required.");
        AssertDynamicProblemExtensionsNotNull(problemDetails);
    }

    public static async Task AssertValidationProblem(HttpContent responseContent, string url, dynamic errors)
    {
        var problemDetails = (await responseContent.ReadFromJsonAsync<ProblemDetails>())!;

        AssertBadRequest(url, problemDetails, "One or more validation errors occurred.");
        AssertErrors(errors, problemDetails);
        AssertDynamicProblemExtensionsNotNull(problemDetails);
    }

    private static void AssertBadRequest(string url, ProblemDetails problemDetails, string title)
    {
        Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.5.1", problemDetails.Type);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal(title, problemDetails.Title);
        // Assert.Equal("The request body was missing or empty.", problemDetails.Detail);
        Assert.Equal(url, problemDetails.Instance);
    }

    private static void AssertDynamicProblemExtensionsNotNull(ProblemDetails problemDetails)
    {
        Assert.NotNull(problemDetails.Extensions["traceId"]);
        Assert.NotNull(problemDetails.Extensions["requestId"]);
    }

    private static void AssertErrors(dynamic errors, ProblemDetails problemDetails)
    {
        var expectedErrors =
            JsonNode.Parse(JsonSerializer.Serialize(errors));
        var actualErrors = JsonNode.Parse(JsonSerializer.Serialize(problemDetails.Extensions["errors"]));

        Assert.True(JsonNode.DeepEquals(expectedErrors, actualErrors));
    }
}
