namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebAllAvailablePluginsResponse(IEnumerable<WebMarketplacePlugin> Data);
