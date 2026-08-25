using System.Text.Json.Serialization;

namespace ScriptBee.Adapters.Auth.Contracts;

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

public class ExternalAuthorizationResponse
{
    [JsonPropertyName("result")]
    public required bool Result { get; init; }
}
