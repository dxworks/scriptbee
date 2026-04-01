namespace ScriptBee.Rest.Contracts;

public record RestGetInstalledPluginsResponse(IEnumerable<RestInstalledPlugin> Data);
