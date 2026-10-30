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
        using var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        return await SendApiRequest(
            factory,
            client => client.PostAsync(endpoint, content, TestContext.Current.CancellationToken)
        );
    }

    public async Task<HttpResponseMessage> PatchApi<T>(
        TestWebApplicationFactory<TStartup> factory,
        T? data = default
    )
    {
        using var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        return await SendApiRequest(
            factory,
            client => client.PatchAsync(endpoint, content, TestContext.Current.CancellationToken)
        );
    }

    public async Task<HttpResponseMessage> DeleteApi(TestWebApplicationFactory<TStartup> factory)
    {
        return await SendApiRequest(factory, client => client.DeleteAsync(endpoint));
    }

    public async Task<HttpResponseMessage> GetApi(TestWebApplicationFactory<TStartup> factory)
    {
        return await SendApiRequest(
            factory,
            client => client.GetAsync(endpoint, TestContext.Current.CancellationToken)
        );
    }

    public async Task<HttpResponseMessage> GetApi(
        string queryParams,
        TestWebApplicationFactory<TStartup> factory
    )
    {
        return await SendApiRequest(factory, client => client.GetAsync(endpoint + queryParams));
    }

    public async Task<HttpResponseMessage> PutApi<T>(
        TestWebApplicationFactory<TStartup> factory,
        T? data = default,
        string mediaType = MediaTypeNames.Application.Json
    )
    {
        using var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            mediaType
        );

        return await SendApiRequest(
            factory,
            client => client.PutAsync(endpoint, content, TestContext.Current.CancellationToken)
        );
    }

    public async Task<HttpResponseMessage> PutApiFormWithFile(
        TestWebApplicationFactory<TStartup> factory,
        Dictionary<string, string> formData,
        Dictionary<string, byte[]> files
    )
    {
        using var multipartContent = new MultipartFormDataContent();

        foreach (var kvp in formData)
        {
            multipartContent.Add(new StringContent(kvp.Value), kvp.Key);
        }

        foreach (var file in files)
        {
            multipartContent.Add(new ByteArrayContent(file.Value), file.Key, file.Key);
        }

        return await SendApiRequest(
            factory,
            client =>
                client.PutAsync(endpoint, multipartContent, TestContext.Current.CancellationToken)
        );
    }

    public async Task<HttpResponseMessage> PostApiFormWithFile(
        TestWebApplicationFactory<TStartup> factory,
        Dictionary<string, string> formData,
        Dictionary<string, byte[]> files
    )
    {
        using var multipartContent = new MultipartFormDataContent();

        foreach (var kvp in formData)
        {
            multipartContent.Add(new StringContent(kvp.Value), kvp.Key);
        }

        foreach (var file in files)
        {
            multipartContent.Add(new ByteArrayContent(file.Value), file.Key, file.Key);
        }

        return await SendApiRequest(
            factory,
            client =>
                client.PostAsync(endpoint, multipartContent, TestContext.Current.CancellationToken)
        );
    }

    private static async Task<HttpResponseMessage> SendApiRequest(
        TestWebApplicationFactory<TStartup> factory,
        Func<HttpClient, Task<HttpResponseMessage>> sendRequest
    )
    {
        await using (factory)
        using (var client = factory.CreateClient())
        {
            return await sendRequest(client);
        }
    }
}
