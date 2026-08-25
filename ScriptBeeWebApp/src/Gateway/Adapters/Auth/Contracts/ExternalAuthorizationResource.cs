using System.Text.Json.Serialization;

namespace ScriptBee.Adapters.Auth.Contracts;

public class ExternalAuthorizationResource
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("role")]
    public string? Role { get; init; }
}
