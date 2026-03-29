namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebInstalledPluginsResponse(IEnumerable<WebInstalledPlugin> Data);
