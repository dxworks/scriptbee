using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace ScriptBee.Gateway.Web.Tests;

public static class ProblemValidationUtils
{
    public static async Task AssertEmptyRequestBodyProblem(HttpContent responseContent, string url)
    {
        var problemDetails = (await responseContent.ReadFromJsonAsync<ProblemDetails>())!;

        AssertBadRequest(url, problemDetails, "Request body is required.", "The request body was missing or empty.");
        AssertDynamicProblemExtensionsNotNull(problemDetails);
    }

    public static async Task AssertValidationProblem(HttpContent responseContent, string url, dynamic errors)
    {
        var problemDetails = (await responseContent.ReadFromJsonAsync<ProblemDetails>())!;

        AssertBadRequest(url, problemDetails, "One or more validation errors occurred.", null);
        AssertErrors(errors, problemDetails);
        AssertDynamicProblemExtensionsNotNull(problemDetails);
    }

    private static void AssertBadRequest(string url, ProblemDetails problemDetails, string title, string? detail)
    {
        problemDetails.Type.ShouldBe("https://tools.ietf.org/html/rfc9110#section-15.5.1");
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
        problemDetails.Title.ShouldBe(title);
        problemDetails.Detail.ShouldBe(detail);
        problemDetails.Instance.ShouldBe(url);
    }

    private static void AssertDynamicProblemExtensionsNotNull(ProblemDetails problemDetails)
    {
        problemDetails.Extensions["traceId"].ShouldNotBeNull();
        problemDetails.Extensions["requestId"].ShouldNotBeNull();
    }

    private static void AssertErrors(dynamic errors, ProblemDetails problemDetails)
    {
        var expectedErrors =
            JsonNode.Parse(JsonSerializer.Serialize(errors));
        var actualErrors = JsonNode.Parse(JsonSerializer.Serialize(problemDetails.Extensions["errors"]));

        Assert.True(JsonNode.DeepEquals(expectedErrors, actualErrors));
    }
}
