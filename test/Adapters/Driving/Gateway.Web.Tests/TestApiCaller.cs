using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests;

public class TestApiCaller<T>(string endpoint, ITestOutputHelper testOutputHelper)
{
    public async Task<HttpResponseMessage> PostApi(List<string> roles, T? data = default)
    {
        using var client = TestWebApplicationFactory<Program>.CreateClient(testOutputHelper, roles);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.TestAuthenticationScheme);

        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await client.PostAsync(endpoint, content);
        return response;
    }

    public async Task<HttpResponseMessage> PostApiWithoutAuthorization()
    {
        using var client = TestWebApplicationFactory<Program>.CreateClient(testOutputHelper, []);

        var content = new StringContent(JsonSerializer.Serialize<CreateProject.WebCreateProjectCommand?>(null),
            Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await client.PostAsync(endpoint, content);
        return response;
    }
}
