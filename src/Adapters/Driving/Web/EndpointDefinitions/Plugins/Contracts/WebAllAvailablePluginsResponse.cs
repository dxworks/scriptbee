namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

public sealed record WebAllAvailablePluginsResponse(IEnumerable<WebMarketplaceProject> Data);
