using System.Net;
using VeriJson;

namespace ScriptBee.Tests.Common;

public static class ResponseAssertionsUtils
{
    public static async Task AssertResponse(
        this HttpResponseMessage response,
        HttpStatusCode statusCode,
        string responsePath
    )
    {
        response.StatusCode.ShouldBe(statusCode);

        await AssertResponseBody(response, responsePath);
    }

    private static async Task AssertResponseBody(HttpResponseMessage response, string responsePath)
    {
        var actualContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var expectedContent = await File.ReadAllTextAsync(
            FilePathAttribute.GetFilePath(responsePath),
            TestContext.Current.CancellationToken
        );
        actualContent.Should().BeEquivalentTo(expectedContent);
    }
}
