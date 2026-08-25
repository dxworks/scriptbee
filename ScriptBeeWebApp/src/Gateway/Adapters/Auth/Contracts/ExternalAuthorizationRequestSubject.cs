using System.Text.Json.Serialization;

namespace ScriptBee.Adapters.Auth.Contracts;

public class ExternalAuthorizationRequestSubject
{
    [JsonPropertyName("user_id")]
    public required string UserId { get; init; }

    [JsonPropertyName("groups")]
    public required List<string> Groups { get; init; }
}
