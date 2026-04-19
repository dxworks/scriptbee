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

        var response = await client.PostAsync(
            endpoint,
            content,
            TestContext.Current.CancellationToken
        );
        return response;
    }

    public async Task<HttpResponseMessage> PatchApi<T>(
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

        var response = await client.PatchAsync(
            endpoint,
            content,
            TestContext.Current.CancellationToken
        );
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

        var response = await client.GetAsync(endpoint, TestContext.Current.CancellationToken);
        return response;
    }

    public async Task<HttpResponseMessage> GetApi(
        string queryParams,
        TestWebApplicationFactory<TStartup> factory
    )
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(endpoint + queryParams);
        return response;
    }

    public async Task<HttpResponseMessage> PutApi<T>(
        TestWebApplicationFactory<TStartup> factory,
        T? data = default,
        string mediaType = MediaTypeNames.Application.Json
    )
    {
        using var client = factory.CreateClient();

        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, mediaType);

        var response = await client.PutAsync(
            endpoint,
            content,
            TestContext.Current.CancellationToken
        );
        return response;
    }

    public async Task<HttpResponseMessage> PutApiFormWithFile(
        TestWebApplicationFactory<TStartup> factory,
        Dictionary<string, string> formData,
        Dictionary<string, byte[]> files
    )
    {
        using var client = factory.CreateClient();

        using var multipartContent = new MultipartFormDataContent();

        foreach (var kvp in formData)
        {
            multipartContent.Add(new StringContent(kvp.Value), kvp.Key);
        }

        foreach (var file in files)
        {
            multipartContent.Add(new ByteArrayContent(file.Value), file.Key, file.Key);
        }

        var response = await client.PutAsync(
            endpoint,
            multipartContent,
            TestContext.Current.CancellationToken
        );
        return response;
    }
}
