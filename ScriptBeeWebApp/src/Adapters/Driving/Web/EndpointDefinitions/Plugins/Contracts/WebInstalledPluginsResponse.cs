namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebInstalledPluginsResponse(IEnumerable<WebMarketplacePlugin> Data);
