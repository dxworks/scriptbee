namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebGatewayPluginsResponse(IEnumerable<WebInstalledGatewayPlugin> Data);
