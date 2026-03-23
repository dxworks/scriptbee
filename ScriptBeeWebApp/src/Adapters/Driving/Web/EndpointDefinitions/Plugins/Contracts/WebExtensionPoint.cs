namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebExtensionPoint(
    string Kind, 
    string? Language = null, 
    string? Extension = null
);
