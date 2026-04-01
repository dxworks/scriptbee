namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public record WebGetInstalledPluginsResponse(IEnumerable<WebInstalledPlugin> Data);
