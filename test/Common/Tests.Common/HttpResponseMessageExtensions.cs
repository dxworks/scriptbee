using System.Net.Http.Json;

namespace ScriptBee.Tests.Common;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> ReadContentAsync<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadFromJsonAsync<T>();
        return content!;
    }
}
