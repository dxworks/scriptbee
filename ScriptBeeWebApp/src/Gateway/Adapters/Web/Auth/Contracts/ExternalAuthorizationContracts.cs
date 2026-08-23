using System.Text.Json.Serialization;

namespace ScriptBee.Web.Auth.Contracts;

public class ExternalAuthorizationRequest
{
    [JsonPropertyName("input")]
    public required ExternalAuthorizationRequestInput Input { get; init; }
}

public class ExternalAuthorizationRequestInput
{
    [JsonPropertyName("subject")]
    public required ExternalAuthorizationRequestSubject Subject { get; init; }

    [JsonPropertyName("action")]
    public required string Action { get; init; }

    [JsonPropertyName("resource")]
    public required ExternalAuthorizationResource Resource { get; init; }
}

public class ExternalAuthorizationRequestSubject
{
    [JsonPropertyName("user_id")]
    public required string UserId { get; init; }

    [JsonPropertyName("groups")]
    public required List<string> Groups { get; init; }
}

public class ExternalAuthorizationResource
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("role")]
    public string? Role { get; init; }
}

public class ExternalAuthorizationResponse
{
    [JsonPropertyName("result")]
    public required bool Result { get; init; }
}
