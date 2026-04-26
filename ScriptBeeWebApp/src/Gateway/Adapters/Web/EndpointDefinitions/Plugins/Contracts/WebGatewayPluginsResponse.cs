namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebGatewayPluginsResponse(IEnumerable<WebInstalledPlugin> Data);
