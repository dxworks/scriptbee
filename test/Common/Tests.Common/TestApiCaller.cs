using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace ScriptBee.Tests.Common;

public class TestApiCaller<TStartup>(string endpoint)
    where TStartup : class
{
    public async Task<HttpResponseMessage> PostApi<T>(
        TestWebApplicationFactory<TStartup> factory,
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

    public async Task<HttpResponseMessage> DeleteApi(TestWebApplicationFactory<TStartup> factory)
    {
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync(endpoint);
        return response;
    }

    public async Task<HttpResponseMessage> GetApi(TestWebApplicationFactory<TStartup> factory)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(endpoint);
        return response;
    }
}
