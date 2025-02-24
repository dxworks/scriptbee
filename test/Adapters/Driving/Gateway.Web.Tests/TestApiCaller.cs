using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace ScriptBee.Gateway.Web.Tests;

public class TestApiCaller(string endpoint)
{
    public async Task<HttpResponseMessage> PostApi<T>(
        TestWebApplicationFactory<Program> factory,
        T? data = default
    )
    {
        using var client = factory.CreateClient();

        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        var response = await client.PostAsync(endpoint, content);
        return response;
    }

    public async Task<HttpResponseMessage> DeleteApi(TestWebApplicationFactory<Program> factory)
    {
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync(endpoint);
        return response;
    }

    public async Task<HttpResponseMessage> GetApi(TestWebApplicationFactory<Program> factory)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(endpoint);
        return response;
    }
}
