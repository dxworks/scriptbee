using System.Text.Json.Serialization;

namespace ScriptBee.Web.Auth.Contracts;

public class PermissionsRequest
{
    [JsonPropertyName("input")]
    public required PermissionsRequestInput Input { get; init; }
}

public class PermissionsRequestInput
{
    [JsonPropertyName("subject")]
    public required ExternalAuthorizationRequestSubject Subject { get; init; }

    [JsonPropertyName("resource")]
    public required ExternalAuthorizationResource Resource { get; init; }
}

public class PermissionsResponse
{
    [JsonPropertyName("result")]
    public required List<string> Permissions { get; init; }
}
