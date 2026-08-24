using System.Text.Json.Serialization;

namespace ScriptBee.Web.Auth.Contracts;

public class RolesResponse
{
    [JsonPropertyName("result")]
    public required List<RoleEntry> Roles { get; init; }
}

public class RoleEntry
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }
}
