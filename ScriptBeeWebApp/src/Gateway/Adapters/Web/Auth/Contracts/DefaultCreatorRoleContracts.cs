using System.Text.Json.Serialization;

namespace ScriptBee.Web.Auth.Contracts;

public class DefaultCreatorRoleResponse
{
    [JsonPropertyName("result")]
    public required string Result { get; init; }
}
